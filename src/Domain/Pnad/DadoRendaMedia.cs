using DadosTabulares.Domain.Tse;

namespace DadosTabulares.Domain.Pnad;

public sealed class DadoRendaMedia
{
    public UF UF { get; }
    public Trimestre Trimestre { get; }
    public decimal RendaMedia { get; }

    public DadoRendaMedia(UF uf, Trimestre trimestre, decimal rendaMedia)
    {
        if (rendaMedia < 0)
            throw new ArgumentException("Renda média não pode ser negativa.", nameof(rendaMedia));

        UF = uf ?? throw new ArgumentNullException(nameof(uf));
        Trimestre = trimestre ?? throw new ArgumentNullException(nameof(trimestre));
        RendaMedia = rendaMedia;
    }
}
