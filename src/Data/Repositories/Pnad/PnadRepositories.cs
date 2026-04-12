using Data.Repositories;
using DadosTabulares.Domain.Pnad;
using DadosTabulares.Domain.Pnad.Repositories;
using DadosTabulares.Domain.Tse;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Pnad;

public sealed class DadoDesempregoRepository(PublicDataDbContext context) : IDadoDesempregoRepository
{
    public async Task<IReadOnlyList<DadoDesemprego>> ObterPorUfAsync(UF uf, CancellationToken ct = default)
        => await context.DadosDesemprego
            .AsNoTracking()
            .Where(x => x.UfSigla == uf.Sigla)
            .OrderByDescending(x => x.TrimestreAno)
            .ThenByDescending(x => x.TrimestreNumero)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task<IReadOnlyList<DadoDesemprego>> ObterPorTrimestreAsync(Trimestre trimestre, CancellationToken ct = default)
        => await context.DadosDesemprego
            .AsNoTracking()
            .Where(x => x.TrimestreAno == trimestre.Ano && x.TrimestreNumero == trimestre.Numero)
            .OrderBy(x => x.UfSigla)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task<IReadOnlyList<DadoDesemprego>> ObterPorUfETrimestreAsync(UF uf, Trimestre trimestre, CancellationToken ct = default)
        => await context.DadosDesemprego
            .AsNoTracking()
            .Where(x => x.UfSigla == uf.Sigla && x.TrimestreAno == trimestre.Ano && x.TrimestreNumero == trimestre.Numero)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task AdicionarEmLoteAsync(IReadOnlyList<DadoDesemprego> dados, CancellationToken ct = default)
    {
        await context.DadosDesemprego.AddRangeAsync(dados.Select(x => x.ToRecord()), ct);
        await context.SaveChangesAsync(ct);
    }
}

public sealed class DadoInformalidadeRepository(PublicDataDbContext context) : IDadoInformalidadeRepository
{
    public async Task<IReadOnlyList<DadoInformalidade>> ObterPorUfAsync(UF uf, CancellationToken ct = default)
        => await context.DadosInformalidade
            .AsNoTracking()
            .Where(x => x.UfSigla == uf.Sigla)
            .OrderByDescending(x => x.TrimestreAno)
            .ThenByDescending(x => x.TrimestreNumero)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task<IReadOnlyList<DadoInformalidade>> ObterPorTrimestreAsync(Trimestre trimestre, CancellationToken ct = default)
        => await context.DadosInformalidade
            .AsNoTracking()
            .Where(x => x.TrimestreAno == trimestre.Ano && x.TrimestreNumero == trimestre.Numero)
            .OrderBy(x => x.UfSigla)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task<IReadOnlyList<DadoInformalidade>> ObterPorUfETrimestreAsync(UF uf, Trimestre trimestre, CancellationToken ct = default)
        => await context.DadosInformalidade
            .AsNoTracking()
            .Where(x => x.UfSigla == uf.Sigla && x.TrimestreAno == trimestre.Ano && x.TrimestreNumero == trimestre.Numero)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task AdicionarEmLoteAsync(IReadOnlyList<DadoInformalidade> dados, CancellationToken ct = default)
    {
        await context.DadosInformalidade.AddRangeAsync(dados.Select(x => x.ToRecord()), ct);
        await context.SaveChangesAsync(ct);
    }
}

public sealed class DadoRendaMediaRepository(PublicDataDbContext context) : IDadoRendaMediaRepository
{
    public async Task<IReadOnlyList<DadoRendaMedia>> ObterPorUfAsync(UF uf, CancellationToken ct = default)
        => await context.DadosRendaMedia
            .AsNoTracking()
            .Where(x => x.UfSigla == uf.Sigla)
            .OrderByDescending(x => x.TrimestreAno)
            .ThenByDescending(x => x.TrimestreNumero)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task<IReadOnlyList<DadoRendaMedia>> ObterPorTrimestreAsync(Trimestre trimestre, CancellationToken ct = default)
        => await context.DadosRendaMedia
            .AsNoTracking()
            .Where(x => x.TrimestreAno == trimestre.Ano && x.TrimestreNumero == trimestre.Numero)
            .OrderBy(x => x.UfSigla)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task<IReadOnlyList<DadoRendaMedia>> ObterPorUfETrimestreAsync(UF uf, Trimestre trimestre, CancellationToken ct = default)
        => await context.DadosRendaMedia
            .AsNoTracking()
            .Where(x => x.UfSigla == uf.Sigla && x.TrimestreAno == trimestre.Ano && x.TrimestreNumero == trimestre.Numero)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task AdicionarEmLoteAsync(IReadOnlyList<DadoRendaMedia> dados, CancellationToken ct = default)
    {
        await context.DadosRendaMedia.AddRangeAsync(dados.Select(x => x.ToRecord()), ct);
        await context.SaveChangesAsync(ct);
    }
}
