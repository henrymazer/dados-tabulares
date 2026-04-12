using Data;
using DadosTabulares.Domain.Pnad;
using Microsoft.EntityFrameworkCore;

namespace Etl.Pnad;

public sealed class PnadIngestionPipeline(PublicDataDbContext context)
{
    private readonly PnadCsvParser _parser = new();
    private readonly PnadTransformer _transformer = new();

    public async Task<int> IngerirAsync(PnadDataset dataset, Stream csv, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(csv);

        return dataset switch
        {
            PnadDataset.Desemprego => await ReplaceAsync(
                context.DadosDesemprego,
                Parse(csv, _transformer.TransformarDesemprego),
                ToRecord,
                ct),
            PnadDataset.Informalidade => await ReplaceAsync(
                context.DadosInformalidade,
                Parse(csv, _transformer.TransformarInformalidade),
                ToRecord,
                ct),
            PnadDataset.RendaMedia => await ReplaceAsync(
                context.DadosRendaMedia,
                Parse(csv, _transformer.TransformarRendaMedia),
                ToRecord,
                ct),
            _ => throw new ArgumentOutOfRangeException(nameof(dataset), dataset, "Dataset PNAD não suportado.")
        };
    }

    private IReadOnlyList<TDomain> Parse<TDomain>(Stream csv, Func<IEnumerable<PnadCsvRow>, IReadOnlyList<TDomain>> transform)
    {
        using var reader = new StreamReader(csv, leaveOpen: true);
        return transform(_parser.Parse(reader));
    }

    private async Task<int> ReplaceAsync<TDomain, TRecord>(
        DbSet<TRecord> table,
        IReadOnlyList<TDomain> items,
        Func<TDomain, TRecord> map,
        CancellationToken ct)
        where TRecord : class
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        await EnsureTrimestresAsync(items, ExtractTrimestre, ct);
        await ReplaceRowsAsync(table, items, map, ct);
        await context.SaveChangesAsync(ct);

        await transaction.CommitAsync(ct);

        return items.Count;
    }

    private async Task EnsureTrimestresAsync<TDomain>(IEnumerable<TDomain> items, Func<TDomain, TrimestreKey> getKey, CancellationToken ct)
    {
        var distinctKeys = items.Select(getKey).Distinct().ToList();

        foreach (var key in distinctKeys)
        {
            var exists = await context.Trimestres.AnyAsync(x => x.Ano == key.Ano && x.Numero == key.Numero, ct);
            if (!exists)
            {
                context.Trimestres.Add(new TrimestreRecord
                {
                    Ano = key.Ano,
                    Numero = key.Numero
                });
            }
        }

        await context.SaveChangesAsync(ct);
    }

    private async Task ReplaceRowsAsync<TDomain, TRecord>(
        DbSet<TRecord> table,
        IReadOnlyList<TDomain> items,
        Func<TDomain, TRecord> map,
        CancellationToken ct)
        where TRecord : class
    {
        var distinctItems = items.DistinctBy(GetPrimaryKey).ToList();

        await table.ExecuteDeleteAsync(ct);
        context.ChangeTracker.Clear();
        await table.AddRangeAsync(distinctItems.Select(map), ct);
        await context.SaveChangesAsync(ct);
    }

    private static TrimestreKey ExtractTrimestre<TDomain>(TDomain domain)
        => domain switch
        {
            DadoDesemprego dado => new TrimestreKey(dado.Trimestre.Ano, dado.Trimestre.Numero),
            DadoInformalidade dado => new TrimestreKey(dado.Trimestre.Ano, dado.Trimestre.Numero),
            DadoRendaMedia dado => new TrimestreKey(dado.Trimestre.Ano, dado.Trimestre.Numero),
            _ => throw new ArgumentOutOfRangeException(nameof(domain), domain, "Entidade PNAD não suportada.")
        };

    private static object GetPrimaryKey<TDomain>(TDomain domain)
        => domain switch
        {
            DadoDesemprego dado => (dado.UF.Sigla, dado.Trimestre.Ano, dado.Trimestre.Numero),
            DadoInformalidade dado => (dado.UF.Sigla, dado.Trimestre.Ano, dado.Trimestre.Numero),
            DadoRendaMedia dado => (dado.UF.Sigla, dado.Trimestre.Ano, dado.Trimestre.Numero),
            _ => throw new ArgumentOutOfRangeException(nameof(domain), domain, "Entidade PNAD não suportada.")
        };

    private static DadoDesempregoRecord ToRecord(DadoDesemprego domain)
        => new()
        {
            UfSigla = domain.UF.Sigla,
            TrimestreAno = domain.Trimestre.Ano,
            TrimestreNumero = domain.Trimestre.Numero,
            TaxaDesemprego = domain.TaxaDesemprego
        };

    private static DadoInformalidadeRecord ToRecord(DadoInformalidade domain)
        => new()
        {
            UfSigla = domain.UF.Sigla,
            TrimestreAno = domain.Trimestre.Ano,
            TrimestreNumero = domain.Trimestre.Numero,
            TaxaInformalidade = domain.TaxaInformalidade
        };

    private static DadoRendaMediaRecord ToRecord(DadoRendaMedia domain)
        => new()
        {
            UfSigla = domain.UF.Sigla,
            TrimestreAno = domain.Trimestre.Ano,
            TrimestreNumero = domain.Trimestre.Numero,
            RendaMedia = domain.RendaMedia
        };

    private sealed record TrimestreKey(int Ano, int Numero);
}
