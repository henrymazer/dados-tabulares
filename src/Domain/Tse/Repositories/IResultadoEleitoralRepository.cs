namespace DadosTabulares.Domain.Tse.Repositories;

public interface IResultadoEleitoralRepository
{
    Task<IReadOnlyList<ResultadoEleitoral>> ObterPorMunicipioAsync(int codigoIbge, AnoEleicao anoEleicao, CancellationToken ct = default);
    Task<IReadOnlyList<ResultadoEleitoral>> ObterPorZonaAsync(int numeroZona, int codigoIbge, AnoEleicao anoEleicao, CancellationToken ct = default);
    Task<IReadOnlyList<ResultadoEleitoral>> ObterPorCandidatoAsync(string cpf, AnoEleicao anoEleicao, CancellationToken ct = default);
    Task<IReadOnlyList<ResultadoEleitoral>> ObterPorPartidoAsync(int numeroPartido, AnoEleicao anoEleicao, CancellationToken ct = default);
    Task AdicionarEmLoteAsync(IReadOnlyList<ResultadoEleitoral> resultados, CancellationToken ct = default);
}
