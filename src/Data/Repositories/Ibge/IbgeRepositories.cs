using Data.Repositories;
using DadosTabulares.Domain.Ibge;
using DadosTabulares.Domain.Ibge.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Ibge;

public sealed class DadoPopulacionalRepository(PublicDataDbContext context) : IDadoPopulacionalRepository
{
    public async Task<IReadOnlyList<DadoPopulacional>> ObterPorMunicipioAsync(int codigoIbge, CancellationToken ct = default)
        => await context.DadosPopulacionais
            .AsNoTracking()
            .Where(x => x.MunicipioCodigoIbge == codigoIbge)
            .OrderByDescending(x => x.Quantidade)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task<IReadOnlyList<DadoPopulacional>> ObterPorUfAsync(string siglaUf, CancellationToken ct = default)
        => await context.DadosPopulacionais
            .AsNoTracking()
            .Where(x => x.UfSigla == siglaUf.ToUpperInvariant())
            .OrderByDescending(x => x.Quantidade)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task AdicionarEmLoteAsync(IReadOnlyList<DadoPopulacional> dados, CancellationToken ct = default)
    {
        await context.DadosPopulacionais.AddRangeAsync(dados.Select(x => x.ToRecord()), ct);
        await context.SaveChangesAsync(ct);
    }
}

public sealed class DadoEscolaridadeRepository(PublicDataDbContext context) : IDadoEscolaridadeRepository
{
    public async Task<IReadOnlyList<DadoEscolaridade>> ObterPorMunicipioAsync(int codigoIbge, CancellationToken ct = default)
        => await context.DadosEscolaridade
            .AsNoTracking()
            .Where(x => x.MunicipioCodigoIbge == codigoIbge)
            .OrderByDescending(x => x.Quantidade)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task<IReadOnlyList<DadoEscolaridade>> ObterPorUfAsync(string siglaUf, CancellationToken ct = default)
        => await context.DadosEscolaridade
            .AsNoTracking()
            .Where(x => x.UfSigla == siglaUf.ToUpperInvariant())
            .OrderByDescending(x => x.Quantidade)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task AdicionarEmLoteAsync(IReadOnlyList<DadoEscolaridade> dados, CancellationToken ct = default)
    {
        await context.DadosEscolaridade.AddRangeAsync(dados.Select(x => x.ToRecord()), ct);
        await context.SaveChangesAsync(ct);
    }
}

public sealed class DadoRendaRepository(PublicDataDbContext context) : IDadoRendaRepository
{
    public async Task<IReadOnlyList<DadoRenda>> ObterPorMunicipioAsync(int codigoIbge, CancellationToken ct = default)
        => await context.DadosRenda
            .AsNoTracking()
            .Where(x => x.MunicipioCodigoIbge == codigoIbge)
            .OrderByDescending(x => x.Quantidade)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task<IReadOnlyList<DadoRenda>> ObterPorUfAsync(string siglaUf, CancellationToken ct = default)
        => await context.DadosRenda
            .AsNoTracking()
            .Where(x => x.UfSigla == siglaUf.ToUpperInvariant())
            .OrderByDescending(x => x.Quantidade)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task AdicionarEmLoteAsync(IReadOnlyList<DadoRenda> dados, CancellationToken ct = default)
    {
        await context.DadosRenda.AddRangeAsync(dados.Select(x => x.ToRecord()), ct);
        await context.SaveChangesAsync(ct);
    }
}

public sealed class DadoSaneamentoRepository(PublicDataDbContext context) : IDadoSaneamentoRepository
{
    public async Task<IReadOnlyList<DadoSaneamento>> ObterPorMunicipioAsync(int codigoIbge, CancellationToken ct = default)
        => await context.DadosSaneamento
            .AsNoTracking()
            .Where(x => x.MunicipioCodigoIbge == codigoIbge)
            .OrderByDescending(x => x.DomiciliosAtendidos)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task<IReadOnlyList<DadoSaneamento>> ObterPorUfAsync(string siglaUf, CancellationToken ct = default)
        => await context.DadosSaneamento
            .AsNoTracking()
            .Where(x => x.UfSigla == siglaUf.ToUpperInvariant())
            .OrderByDescending(x => x.DomiciliosAtendidos)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task AdicionarEmLoteAsync(IReadOnlyList<DadoSaneamento> dados, CancellationToken ct = default)
    {
        await context.DadosSaneamento.AddRangeAsync(dados.Select(x => x.ToRecord()), ct);
        await context.SaveChangesAsync(ct);
    }
}

public sealed class DadoUrbanizacaoRepository(PublicDataDbContext context) : IDadoUrbanizacaoRepository
{
    public async Task<IReadOnlyList<DadoUrbanizacao>> ObterPorMunicipioAsync(int codigoIbge, CancellationToken ct = default)
        => await context.DadosUrbanizacao
            .AsNoTracking()
            .Where(x => x.MunicipioCodigoIbge == codigoIbge)
            .OrderByDescending(x => x.Populacao)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task<IReadOnlyList<DadoUrbanizacao>> ObterPorUfAsync(string siglaUf, CancellationToken ct = default)
        => await context.DadosUrbanizacao
            .AsNoTracking()
            .Where(x => x.UfSigla == siglaUf.ToUpperInvariant())
            .OrderByDescending(x => x.Populacao)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task AdicionarEmLoteAsync(IReadOnlyList<DadoUrbanizacao> dados, CancellationToken ct = default)
    {
        await context.DadosUrbanizacao.AddRangeAsync(dados.Select(x => x.ToRecord()), ct);
        await context.SaveChangesAsync(ct);
    }
}

public sealed class DadoInfraestruturaRepository(PublicDataDbContext context) : IDadoInfraestruturaRepository
{
    public async Task<IReadOnlyList<DadoInfraestrutura>> ObterPorMunicipioAsync(int codigoIbge, CancellationToken ct = default)
        => await context.DadosInfraestrutura
            .AsNoTracking()
            .Where(x => x.MunicipioCodigoIbge == codigoIbge)
            .OrderByDescending(x => x.DomiciliosAtendidos)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task<IReadOnlyList<DadoInfraestrutura>> ObterPorUfAsync(string siglaUf, CancellationToken ct = default)
        => await context.DadosInfraestrutura
            .AsNoTracking()
            .Where(x => x.UfSigla == siglaUf.ToUpperInvariant())
            .OrderByDescending(x => x.DomiciliosAtendidos)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task AdicionarEmLoteAsync(IReadOnlyList<DadoInfraestrutura> dados, CancellationToken ct = default)
    {
        await context.DadosInfraestrutura.AddRangeAsync(dados.Select(x => x.ToRecord()), ct);
        await context.SaveChangesAsync(ct);
    }
}
