using DadosTabulares.Domain.Tse;

namespace DadosTabulares.Domain.Pnad;

public sealed class DadoInformalidade
{
    public UF UF { get; }
    public Trimestre Trimestre { get; }
    public decimal TaxaInformalidade { get; }

    public DadoInformalidade(UF uf, Trimestre trimestre, decimal taxaInformalidade)
    {
        if (taxaInformalidade < 0)
            throw new ArgumentException("Taxa de informalidade não pode ser negativa.", nameof(taxaInformalidade));

        UF = uf ?? throw new ArgumentNullException(nameof(uf));
        Trimestre = trimestre ?? throw new ArgumentNullException(nameof(trimestre));
        TaxaInformalidade = taxaInformalidade;
    }
}
