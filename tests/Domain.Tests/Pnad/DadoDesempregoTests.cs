namespace Domain.Tests.Pnad;

using DadosTabulares.Domain.Pnad;
using DadosTabulares.Domain.Tse;

public class DadoDesempregoTests
{
    private static UF CriarUf() => new("SP");

    private static Trimestre CriarTrimestre() => new(2024, 1);

    [Fact]
    public void Deve_criar_dado_desemprego_valido()
    {
        var uf = CriarUf();
        var trimestre = CriarTrimestre();

        var dado = new DadoDesemprego(
            uf: uf,
            trimestre: trimestre,
            taxaDesemprego: 8.5m);

        Assert.Equal(uf, dado.UF);
        Assert.Equal(trimestre, dado.Trimestre);
        Assert.Equal(8.5m, dado.TaxaDesemprego);
    }

    [Fact]
    public void Deve_rejeitar_uf_nula()
    {
        Assert.Throws<ArgumentNullException>(() => new DadoDesemprego(
            uf: null!,
            trimestre: CriarTrimestre(),
            taxaDesemprego: 8.5m));
    }

    [Fact]
    public void Deve_rejeitar_trimestre_nulo()
    {
        Assert.Throws<ArgumentNullException>(() => new DadoDesemprego(
            uf: CriarUf(),
            trimestre: null!,
            taxaDesemprego: 8.5m));
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(-10)]
    public void Deve_rejeitar_taxa_desemprego_negativa(decimal taxaDesemprego)
    {
        Assert.Throws<ArgumentException>(() => new DadoDesemprego(
            uf: CriarUf(),
            trimestre: CriarTrimestre(),
            taxaDesemprego: taxaDesemprego));
    }

    [Fact]
    public void Deve_aceitar_taxa_desemprego_zero()
    {
        var dado = new DadoDesemprego(
            uf: CriarUf(),
            trimestre: CriarTrimestre(),
            taxaDesemprego: 0m);

        Assert.Equal(0m, dado.TaxaDesemprego);
    }
}
