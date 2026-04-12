namespace DadosTabulares.Domain.Tse;

public sealed record AnoEleicao
{
    public int Ano { get; }

    public AnoEleicao(int ano)
    {
        if (ano <= 0 || ano % 2 != 0)
            throw new ArgumentException($"Ano {ano} não é um ano eleitoral válido. Eleições ocorrem em anos pares.", nameof(ano));

        Ano = ano;
    }

    public override string ToString() => Ano.ToString();
}
