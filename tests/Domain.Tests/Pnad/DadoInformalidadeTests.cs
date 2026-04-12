namespace Domain.Tests.Pnad;

using DadosTabulares.Domain.Pnad;
using DadosTabulares.Domain.Tse;

public class DadoInformalidadeTests
{
    private static UF CriarUf() => new("SP");

    private static Trimestre CriarTrimestre() => new(2024, 1);

    [Fact]
    public void Deve_criar_dado_informalidade_valido()
    {
        var uf = CriarUf();
        var trimestre = CriarTrimestre();

        var dado = new DadoInformalidade(
            uf: uf,
            trimestre: trimestre,
            taxaInformalidade: 37.25m);

        Assert.Equal(uf, dado.UF);
        Assert.Equal(trimestre, dado.Trimestre);
        Assert.Equal(37.25m, dado.TaxaInformalidade);
    }

    [Fact]
    public void Deve_rejeitar_uf_nula()
    {
        Assert.Throws<ArgumentNullException>(() => new DadoInformalidade(
            uf: null!,
            trimestre: CriarTrimestre(),
            taxaInformalidade: 37.25m));
    }

    [Fact]
    public void Deve_rejeitar_trimestre_nulo()
    {
        Assert.Throws<ArgumentNullException>(() => new DadoInformalidade(
            uf: CriarUf(),
            trimestre: null!,
            taxaInformalidade: 37.25m));
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-10)]
    public void Deve_rejeitar_taxa_informalidade_negativa(decimal taxaInformalidade)
    {
        Assert.Throws<ArgumentException>(() => new DadoInformalidade(
            uf: CriarUf(),
            trimestre: CriarTrimestre(),
            taxaInformalidade: taxaInformalidade));
    }

    [Fact]
    public void Deve_aceitar_taxa_informalidade_zero()
    {
        var dado = new DadoInformalidade(
            uf: CriarUf(),
            trimestre: CriarTrimestre(),
            taxaInformalidade: 0m);

        Assert.Equal(0m, dado.TaxaInformalidade);
    }
}
