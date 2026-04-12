namespace DadosTabulares.Domain.Tse.Repositories;

public interface IBensDeclaradosRepository
{
    Task<IReadOnlyList<BemDeclarado>> ObterPorCandidatoAsync(string cpf, AnoEleicao anoEleicao, CancellationToken ct = default);
    Task AdicionarEmLoteAsync(IReadOnlyList<BemDeclarado> bens, CancellationToken ct = default);
}
