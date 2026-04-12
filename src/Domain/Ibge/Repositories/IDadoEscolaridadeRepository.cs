namespace DadosTabulares.Domain.Ibge.Repositories;

public interface IDadoEscolaridadeRepository
{
    Task<IReadOnlyList<DadoEscolaridade>> ObterPorMunicipioAsync(int codigoIbge, CancellationToken ct = default);
    Task<IReadOnlyList<DadoEscolaridade>> ObterPorUfAsync(string siglaUf, CancellationToken ct = default);
    Task AdicionarEmLoteAsync(IReadOnlyList<DadoEscolaridade> dados, CancellationToken ct = default);
}
