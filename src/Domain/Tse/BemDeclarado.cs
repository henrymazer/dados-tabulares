namespace DadosTabulares.Domain.Tse;

public class BemDeclarado
{
    public Candidato Candidato { get; }
    public AnoEleicao AnoEleicao { get; }
    public string TipoBem { get; }
    public string Descricao { get; }
    public decimal Valor { get; }

    public BemDeclarado(
        Candidato candidato,
        AnoEleicao anoEleicao,
        string tipoBem,
        string descricao,
        decimal valor)
    {
        if (string.IsNullOrWhiteSpace(tipoBem))
            throw new ArgumentException("Tipo do bem não pode ser vazio.", nameof(tipoBem));

        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição do bem não pode ser vazia.", nameof(descricao));

        if (valor < 0)
            throw new ArgumentException("Valor do bem não pode ser negativo.", nameof(valor));

        Candidato = candidato ?? throw new ArgumentNullException(nameof(candidato));
        AnoEleicao = anoEleicao ?? throw new ArgumentNullException(nameof(anoEleicao));
        TipoBem = tipoBem;
        Descricao = descricao;
        Valor = valor;
    }
}
