using DadosTabulares.Domain.Tse;

namespace DadosTabulares.Domain.Pnad.Repositories;

public interface IDadoRendaMediaRepository
{
    Task<IReadOnlyList<DadoRendaMedia>> ObterPorUfAsync(UF uf, CancellationToken ct = default);
    Task<IReadOnlyList<DadoRendaMedia>> ObterPorTrimestreAsync(Trimestre trimestre, CancellationToken ct = default);
    Task<IReadOnlyList<DadoRendaMedia>> ObterPorUfETrimestreAsync(UF uf, Trimestre trimestre, CancellationToken ct = default);
    Task AdicionarEmLoteAsync(IReadOnlyList<DadoRendaMedia> dados, CancellationToken ct = default);
}
