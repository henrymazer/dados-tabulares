namespace DadosTabulares.Domain.Tse;

public sealed record Municipio
{
    public int CodigoIbge { get; }
    public string Nome { get; }
    public UF UF { get; }

    public Municipio(int codigoIbge, string nome, UF uf)
    {
        if (codigoIbge <= 0)
            throw new ArgumentException("Código IBGE deve ser maior que zero.", nameof(codigoIbge));

        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do município não pode ser vazio.", nameof(nome));

        CodigoIbge = codigoIbge;
        Nome = nome;
        UF = uf ?? throw new ArgumentNullException(nameof(uf));
    }

    public override string ToString() => $"{Nome}/{UF.Sigla}";
}
