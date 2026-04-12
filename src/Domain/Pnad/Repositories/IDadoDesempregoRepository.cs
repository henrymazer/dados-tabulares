using DadosTabulares.Domain.Tse;

namespace DadosTabulares.Domain.Pnad.Repositories;

public interface IDadoDesempregoRepository
{
    Task<IReadOnlyList<DadoDesemprego>> ObterPorUfAsync(UF uf, CancellationToken ct = default);
    Task<IReadOnlyList<DadoDesemprego>> ObterPorTrimestreAsync(Trimestre trimestre, CancellationToken ct = default);
    Task<IReadOnlyList<DadoDesemprego>> ObterPorUfETrimestreAsync(UF uf, Trimestre trimestre, CancellationToken ct = default);
    Task AdicionarEmLoteAsync(IReadOnlyList<DadoDesemprego> dados, CancellationToken ct = default);
}
