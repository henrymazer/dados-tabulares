using Data;
using DadosTabulares.Domain.Ibge;
using Microsoft.EntityFrameworkCore;

namespace Etl.Ibge;

public sealed class IbgeIngestionPipeline(PublicDataDbContext context)
{
    private readonly IbgeCsvParser _parser = new();

    public async Task<int> IngerirAsync(IbgeDataset dataset, Stream csv, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(csv);

        return dataset switch
        {
            IbgeDataset.Populacional => await ReplaceAsync(context.DadosPopulacionais, _parser.ParsePopulacional(csv), ToRecord, ct),
            IbgeDataset.Escolaridade => await ReplaceAsync(context.DadosEscolaridade, _parser.ParseEscolaridade(csv), ToRecord, ct),
            IbgeDataset.Renda => await ReplaceAsync(context.DadosRenda, _parser.ParseRenda(csv), ToRecord, ct),
            IbgeDataset.Saneamento => await ReplaceAsync(context.DadosSaneamento, _parser.ParseSaneamento(csv), ToRecord, ct),
            IbgeDataset.Urbanizacao => await ReplaceAsync(context.DadosUrbanizacao, _parser.ParseUrbanizacao(csv), ToRecord, ct),
            IbgeDataset.Infraestrutura => await ReplaceAsync(context.DadosInfraestrutura, _parser.ParseInfraestrutura(csv), ToRecord, ct),
            _ => throw new ArgumentOutOfRangeException(nameof(dataset), dataset, "Dataset IBGE não suportado.")
        };
    }

    private async Task<int> ReplaceAsync<TDomain, TRecord>(
        DbSet<TRecord> table,
        IReadOnlyList<TDomain> items,
        Func<TDomain, TRecord> map,
        CancellationToken ct)
        where TRecord : class
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        await table.ExecuteDeleteAsync(ct);
        context.ChangeTracker.Clear();
        await table.AddRangeAsync(items.Select(map), ct);
        await context.SaveChangesAsync(ct);

        await transaction.CommitAsync(ct);

        return items.Count;
    }

    private static DadoPopulacionalRecord ToRecord(DadoPopulacional domain)
        => new()
        {
            MunicipioCodigoIbge = domain.Municipio.CodigoIbge,
            MunicipioNome = domain.Municipio.Nome,
            UfSigla = domain.Municipio.UF.Sigla,
            FaixaEtaria = domain.FaixaEtaria,
            Raca = domain.Raca,
            Quantidade = domain.Quantidade
        };

    private static DadoEscolaridadeRecord ToRecord(DadoEscolaridade domain)
        => new()
        {
            MunicipioCodigoIbge = domain.Municipio.CodigoIbge,
            MunicipioNome = domain.Municipio.Nome,
            UfSigla = domain.Municipio.UF.Sigla,
            NivelEscolaridade = domain.NivelEscolaridade,
            Quantidade = domain.Quantidade
        };

    private static DadoRendaRecord ToRecord(DadoRenda domain)
        => new()
        {
            MunicipioCodigoIbge = domain.Municipio.CodigoIbge,
            MunicipioNome = domain.Municipio.Nome,
            UfSigla = domain.Municipio.UF.Sigla,
            FaixaRenda = domain.FaixaRenda,
            Quantidade = domain.Quantidade
        };

    private static DadoSaneamentoRecord ToRecord(DadoSaneamento domain)
        => new()
        {
            MunicipioCodigoIbge = domain.Municipio.CodigoIbge,
            MunicipioNome = domain.Municipio.Nome,
            UfSigla = domain.Municipio.UF.Sigla,
            TipoSaneamento = domain.TipoSaneamento,
            DomiciliosAtendidos = domain.DomiciliosAtendidos
        };

    private static DadoUrbanizacaoRecord ToRecord(DadoUrbanizacao domain)
        => new()
        {
            MunicipioCodigoIbge = domain.Municipio.CodigoIbge,
            MunicipioNome = domain.Municipio.Nome,
            UfSigla = domain.Municipio.UF.Sigla,
            TipoArea = domain.TipoArea,
            Populacao = domain.Populacao
        };

    private static DadoInfraestruturaRecord ToRecord(DadoInfraestrutura domain)
        => new()
        {
            MunicipioCodigoIbge = domain.Municipio.CodigoIbge,
            MunicipioNome = domain.Municipio.Nome,
            UfSigla = domain.Municipio.UF.Sigla,
            TipoInfraestrutura = domain.TipoInfraestrutura,
            DomiciliosAtendidos = domain.DomiciliosAtendidos
        };
}
