namespace DadosTabulares.Domain.Tse;

public class PrestacaoContas
{
    public Candidato Candidato { get; }
    public AnoEleicao AnoEleicao { get; }
    public string TipoReceita { get; }
    public string Descricao { get; }
    public decimal Valor { get; }
    public string TipoMovimentacao { get; }

    public PrestacaoContas(
        Candidato candidato,
        AnoEleicao anoEleicao,
        string tipoReceita,
        string descricao,
        decimal valor,
        string tipoMovimentacao)
    {
        if (string.IsNullOrWhiteSpace(tipoReceita))
            throw new ArgumentException("Tipo de receita não pode ser vazio.", nameof(tipoReceita));

        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição não pode ser vazia.", nameof(descricao));

        if (valor < 0)
            throw new ArgumentException("Valor não pode ser negativo.", nameof(valor));

        if (string.IsNullOrWhiteSpace(tipoMovimentacao))
            throw new ArgumentException("Tipo de movimentação não pode ser vazio.", nameof(tipoMovimentacao));

        Candidato = candidato ?? throw new ArgumentNullException(nameof(candidato));
        AnoEleicao = anoEleicao ?? throw new ArgumentNullException(nameof(anoEleicao));
        TipoReceita = tipoReceita;
        Descricao = descricao;
        Valor = valor;
        TipoMovimentacao = tipoMovimentacao;
    }
}
