using Data.Repositories;
using DadosTabulares.Domain.Tse;
using DadosTabulares.Domain.Tse.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Tse;

public sealed class ResultadoEleitoralRepository(PublicDataDbContext context) : IResultadoEleitoralRepository
{
    public async Task<IReadOnlyList<ResultadoEleitoral>> ObterPorMunicipioAsync(int codigoIbge, AnoEleicao anoEleicao, CancellationToken ct = default)
        => await context.ResultadosEleitorais
            .AsNoTracking()
            .Where(x => x.MunicipioCodigoIbge == codigoIbge && x.AnoEleicao == anoEleicao.Ano)
            .OrderByDescending(x => x.QuantidadeVotos)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ResultadoEleitoral>> ObterPorZonaAsync(int numeroZona, int codigoIbge, AnoEleicao anoEleicao, CancellationToken ct = default)
        => await context.ResultadosEleitorais
            .AsNoTracking()
            .Where(x => x.NumeroZona == numeroZona && x.MunicipioCodigoIbge == codigoIbge && x.AnoEleicao == anoEleicao.Ano)
            .OrderByDescending(x => x.QuantidadeVotos)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ResultadoEleitoral>> ObterPorCandidatoAsync(string cpf, AnoEleicao anoEleicao, CancellationToken ct = default)
        => await context.ResultadosEleitorais
            .AsNoTracking()
            .Where(x => x.CandidatoCpf == cpf && x.AnoEleicao == anoEleicao.Ano)
            .OrderByDescending(x => x.QuantidadeVotos)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ResultadoEleitoral>> ObterPorPartidoAsync(int numeroPartido, AnoEleicao anoEleicao, CancellationToken ct = default)
        => await context.ResultadosEleitorais
            .AsNoTracking()
            .Where(x => x.PartidoNumero == numeroPartido && x.AnoEleicao == anoEleicao.Ano)
            .OrderByDescending(x => x.QuantidadeVotos)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task AdicionarEmLoteAsync(IReadOnlyList<ResultadoEleitoral> resultados, CancellationToken ct = default)
    {
        await context.ResultadosEleitorais.AddRangeAsync(resultados.Select(x => x.ToRecord()), ct);
        await context.SaveChangesAsync(ct);
    }
}

public sealed class PerfilEleitoradoRepository(PublicDataDbContext context) : IPerfilEleitoradoRepository
{
    public async Task<IReadOnlyList<PerfilEleitorado>> ObterPorZonaAsync(int numeroZona, int codigoIbge, AnoEleicao anoEleicao, CancellationToken ct = default)
        => await context.PerfisEleitorado
            .AsNoTracking()
            .Where(x => x.NumeroZona == numeroZona && x.MunicipioCodigoIbge == codigoIbge && x.AnoEleicao == anoEleicao.Ano)
            .OrderByDescending(x => x.QuantidadeEleitores)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task<IReadOnlyList<PerfilEleitorado>> ObterPorMunicipioAsync(int codigoIbge, AnoEleicao anoEleicao, CancellationToken ct = default)
        => await context.PerfisEleitorado
            .AsNoTracking()
            .Where(x => x.MunicipioCodigoIbge == codigoIbge && x.AnoEleicao == anoEleicao.Ano)
            .OrderByDescending(x => x.QuantidadeEleitores)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task AdicionarEmLoteAsync(IReadOnlyList<PerfilEleitorado> perfis, CancellationToken ct = default)
    {
        await context.PerfisEleitorado.AddRangeAsync(perfis.Select(x => x.ToRecord()), ct);
        await context.SaveChangesAsync(ct);
    }
}

public sealed class BensDeclaradosRepository(PublicDataDbContext context) : IBensDeclaradosRepository
{
    public async Task<IReadOnlyList<BemDeclarado>> ObterPorCandidatoAsync(string cpf, AnoEleicao anoEleicao, CancellationToken ct = default)
        => await context.BensDeclarados
            .AsNoTracking()
            .Where(x => x.CandidatoCpf == cpf && x.AnoEleicao == anoEleicao.Ano)
            .OrderByDescending(x => x.Valor)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task AdicionarEmLoteAsync(IReadOnlyList<BemDeclarado> bens, CancellationToken ct = default)
    {
        await context.BensDeclarados.AddRangeAsync(bens.Select(x => x.ToRecord()), ct);
        await context.SaveChangesAsync(ct);
    }
}

public sealed class PrestacaoContasRepository(PublicDataDbContext context) : IPrestacaoContasRepository
{
    public async Task<IReadOnlyList<PrestacaoContas>> ObterPorCandidatoAsync(string cpf, AnoEleicao anoEleicao, CancellationToken ct = default)
        => await context.PrestacoesContas
            .AsNoTracking()
            .Where(x => x.CandidatoCpf == cpf && x.AnoEleicao == anoEleicao.Ano)
            .OrderByDescending(x => x.Valor)
            .Select(x => x.ToDomain())
            .ToListAsync(ct);

    public async Task AdicionarEmLoteAsync(IReadOnlyList<PrestacaoContas> prestacoes, CancellationToken ct = default)
    {
        await context.PrestacoesContas.AddRangeAsync(prestacoes.Select(x => x.ToRecord()), ct);
        await context.SaveChangesAsync(ct);
    }
}
