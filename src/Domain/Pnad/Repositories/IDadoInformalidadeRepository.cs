using DadosTabulares.Domain.Tse;

namespace DadosTabulares.Domain.Pnad.Repositories;

public interface IDadoInformalidadeRepository
{
    Task<IReadOnlyList<DadoInformalidade>> ObterPorUfAsync(UF uf, CancellationToken ct = default);
    Task<IReadOnlyList<DadoInformalidade>> ObterPorTrimestreAsync(Trimestre trimestre, CancellationToken ct = default);
    Task<IReadOnlyList<DadoInformalidade>> ObterPorUfETrimestreAsync(UF uf, Trimestre trimestre, CancellationToken ct = default);
    Task AdicionarEmLoteAsync(IReadOnlyList<DadoInformalidade> dados, CancellationToken ct = default);
}
