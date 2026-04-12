namespace DadosTabulares.Domain.Ibge.Repositories;

public interface IDadoPopulacionalRepository
{
    Task<IReadOnlyList<DadoPopulacional>> ObterPorMunicipioAsync(int codigoIbge, CancellationToken ct = default);
    Task<IReadOnlyList<DadoPopulacional>> ObterPorUfAsync(string siglaUf, CancellationToken ct = default);
    Task AdicionarEmLoteAsync(IReadOnlyList<DadoPopulacional> dados, CancellationToken ct = default);
}
