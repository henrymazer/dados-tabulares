namespace DadosTabulares.Domain.Tse;

public sealed record UF
{
    private static readonly HashSet<string> SiglasValidas =
    [
        "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO",
        "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI",
        "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO", "ZZ"
    ];

    public string Sigla { get; }

    public UF(string sigla)
    {
        if (string.IsNullOrWhiteSpace(sigla))
            throw new ArgumentException("Sigla da UF não pode ser vazia.", nameof(sigla));

        var normalizada = sigla.ToUpperInvariant();

        if (!SiglasValidas.Contains(normalizada))
            throw new ArgumentException($"'{sigla}' não é uma UF válida.", nameof(sigla));

        Sigla = normalizada;
    }

    public override string ToString() => Sigla;
}
