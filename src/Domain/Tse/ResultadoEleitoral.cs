namespace DadosTabulares.Domain.Tse;

public class ResultadoEleitoral
{
    public Candidato Candidato { get; }
    public AnoEleicao AnoEleicao { get; }
    public int Turno { get; }
    public ZonaEleitoral ZonaEleitoral { get; }
    public SecaoEleitoral SecaoEleitoral { get; }
    public int QuantidadeVotos { get; }

    public ResultadoEleitoral(
        Candidato candidato,
        AnoEleicao anoEleicao,
        int turno,
        ZonaEleitoral zonaEleitoral,
        SecaoEleitoral secaoEleitoral,
        int quantidadeVotos)
    {
        if (turno is not (1 or 2))
            throw new ArgumentException("Turno deve ser 1 ou 2.", nameof(turno));

        if (quantidadeVotos < 0)
            throw new ArgumentException("Quantidade de votos não pode ser negativa.", nameof(quantidadeVotos));

        Candidato = candidato ?? throw new ArgumentNullException(nameof(candidato));
        AnoEleicao = anoEleicao ?? throw new ArgumentNullException(nameof(anoEleicao));
        Turno = turno;
        ZonaEleitoral = zonaEleitoral ?? throw new ArgumentNullException(nameof(zonaEleitoral));
        SecaoEleitoral = secaoEleitoral ?? throw new ArgumentNullException(nameof(secaoEleitoral));
        QuantidadeVotos = quantidadeVotos;
    }
}
