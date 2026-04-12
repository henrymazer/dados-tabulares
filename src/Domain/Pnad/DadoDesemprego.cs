using DadosTabulares.Domain.Tse;

namespace DadosTabulares.Domain.Pnad;

public sealed class DadoDesemprego
{
    public UF UF { get; }
    public Trimestre Trimestre { get; }
    public decimal TaxaDesemprego { get; }

    public DadoDesemprego(UF uf, Trimestre trimestre, decimal taxaDesemprego)
    {
        if (taxaDesemprego < 0)
            throw new ArgumentException("Taxa de desemprego não pode ser negativa.", nameof(taxaDesemprego));

        UF = uf ?? throw new ArgumentNullException(nameof(uf));
        Trimestre = trimestre ?? throw new ArgumentNullException(nameof(trimestre));
        TaxaDesemprego = taxaDesemprego;
    }
}
