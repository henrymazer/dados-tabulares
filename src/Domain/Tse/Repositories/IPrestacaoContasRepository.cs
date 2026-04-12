namespace DadosTabulares.Domain.Tse.Repositories;

public interface IPrestacaoContasRepository
{
    Task<IReadOnlyList<PrestacaoContas>> ObterPorCandidatoAsync(string cpf, AnoEleicao anoEleicao, CancellationToken ct = default);
    Task AdicionarEmLoteAsync(IReadOnlyList<PrestacaoContas> prestacoes, CancellationToken ct = default);
}
