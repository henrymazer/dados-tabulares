namespace DadosTabulares.Domain.Ibge.Repositories;

public interface IDadoUrbanizacaoRepository
{
    Task<IReadOnlyList<DadoUrbanizacao>> ObterPorMunicipioAsync(int codigoIbge, CancellationToken ct = default);
    Task<IReadOnlyList<DadoUrbanizacao>> ObterPorUfAsync(string siglaUf, CancellationToken ct = default);
    Task AdicionarEmLoteAsync(IReadOnlyList<DadoUrbanizacao> dados, CancellationToken ct = default);
}
