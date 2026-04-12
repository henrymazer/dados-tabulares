namespace DadosTabulares.Domain.Tse.Repositories;

public interface IPerfilEleitoradoRepository
{
    Task<IReadOnlyList<PerfilEleitorado>> ObterPorZonaAsync(int numeroZona, int codigoIbge, AnoEleicao anoEleicao, CancellationToken ct = default);
    Task<IReadOnlyList<PerfilEleitorado>> ObterPorMunicipioAsync(int codigoIbge, AnoEleicao anoEleicao, CancellationToken ct = default);
    Task AdicionarEmLoteAsync(IReadOnlyList<PerfilEleitorado> perfis, CancellationToken ct = default);
}
