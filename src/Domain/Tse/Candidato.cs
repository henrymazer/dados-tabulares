namespace DadosTabulares.Domain.Tse;

public class Candidato
{
    public string Cpf { get; }
    public string Nome { get; }
    public string NomeUrna { get; }
    public int Numero { get; }
    public string Cargo { get; }
    public Partido Partido { get; }
    public AnoEleicao AnoEleicao { get; }
    public UF UF { get; }

    public Candidato(
        string cpf,
        string nome,
        string nomeUrna,
        int numero,
        string cargo,
        Partido partido,
        AnoEleicao anoEleicao,
        UF uf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("CPF do candidato não pode ser vazio.", nameof(cpf));

        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do candidato não pode ser vazio.", nameof(nome));

        if (numero <= 0)
            throw new ArgumentException("Número do candidato deve ser maior que zero.", nameof(numero));

        if (string.IsNullOrWhiteSpace(cargo))
            throw new ArgumentException("Cargo do candidato não pode ser vazio.", nameof(cargo));

        Cpf = cpf;
        Nome = nome;
        NomeUrna = nomeUrna ?? string.Empty;
        Numero = numero;
        Cargo = cargo;
        Partido = partido ?? throw new ArgumentNullException(nameof(partido));
        AnoEleicao = anoEleicao ?? throw new ArgumentNullException(nameof(anoEleicao));
        UF = uf ?? throw new ArgumentNullException(nameof(uf));
    }
}
