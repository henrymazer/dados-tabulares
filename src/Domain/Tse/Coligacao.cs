namespace DadosTabulares.Domain.Tse;

public class Coligacao
{
    public string Nome { get; }
    public AnoEleicao AnoEleicao { get; }
    public IReadOnlyList<Partido> Partidos { get; }

    public Coligacao(string nome, AnoEleicao anoEleicao, IReadOnlyList<Partido> partidos)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome da coligação não pode ser vazio.", nameof(nome));

        ArgumentNullException.ThrowIfNull(anoEleicao);

        if (partidos is null || partidos.Count == 0)
            throw new ArgumentException("Coligação deve ter ao menos um partido.", nameof(partidos));

        Nome = nome;
        AnoEleicao = anoEleicao;
        Partidos = partidos;
    }
}
