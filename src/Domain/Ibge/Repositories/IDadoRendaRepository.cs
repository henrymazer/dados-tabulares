namespace DadosTabulares.Domain.Ibge.Repositories;

public interface IDadoRendaRepository
{
    Task<IReadOnlyList<DadoRenda>> ObterPorMunicipioAsync(int codigoIbge, CancellationToken ct = default);
    Task<IReadOnlyList<DadoRenda>> ObterPorUfAsync(string siglaUf, CancellationToken ct = default);
    Task AdicionarEmLoteAsync(IReadOnlyList<DadoRenda> dados, CancellationToken ct = default);
}
