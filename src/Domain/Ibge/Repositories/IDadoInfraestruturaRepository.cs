namespace DadosTabulares.Domain.Ibge.Repositories;

public interface IDadoInfraestruturaRepository
{
    Task<IReadOnlyList<DadoInfraestrutura>> ObterPorMunicipioAsync(int codigoIbge, CancellationToken ct = default);
    Task<IReadOnlyList<DadoInfraestrutura>> ObterPorUfAsync(string siglaUf, CancellationToken ct = default);
    Task AdicionarEmLoteAsync(IReadOnlyList<DadoInfraestrutura> dados, CancellationToken ct = default);
}
