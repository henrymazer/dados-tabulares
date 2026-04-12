namespace DadosTabulares.Domain.Pnad;

public sealed record Trimestre
{
    public int Ano { get; }
    public int Numero { get; }

    public Trimestre(int ano, int numero)
    {
        if (ano <= 0)
            throw new ArgumentException("Ano do trimestre deve ser maior que zero.", nameof(ano));

        if (numero is < 1 or > 4)
            throw new ArgumentException("Número do trimestre deve estar entre 1 e 4.", nameof(numero));

        Ano = ano;
        Numero = numero;
    }

    public override string ToString() => $"{Ano}-T{Numero}";
}
