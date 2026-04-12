namespace DadosTabulares.Domain.Ibge.Repositories;

public interface IDadoSaneamentoRepository
{
    Task<IReadOnlyList<DadoSaneamento>> ObterPorMunicipioAsync(int codigoIbge, CancellationToken ct = default);
    Task<IReadOnlyList<DadoSaneamento>> ObterPorUfAsync(string siglaUf, CancellationToken ct = default);
    Task AdicionarEmLoteAsync(IReadOnlyList<DadoSaneamento> dados, CancellationToken ct = default);
}
