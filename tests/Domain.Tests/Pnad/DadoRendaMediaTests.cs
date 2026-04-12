namespace Domain.Tests.Pnad;

using DadosTabulares.Domain.Pnad;
using DadosTabulares.Domain.Tse;

public class DadoRendaMediaTests
{
    private static UF CriarUf() => new("SP");

    private static Trimestre CriarTrimestre() => new(2024, 1);

    [Fact]
    public void Deve_criar_dado_renda_media_valido()
    {
        var uf = CriarUf();
        var trimestre = CriarTrimestre();

        var dado = new DadoRendaMedia(
            uf: uf,
            trimestre: trimestre,
            rendaMedia: 3250.75m);

        Assert.Equal(uf, dado.UF);
        Assert.Equal(trimestre, dado.Trimestre);
        Assert.Equal(3250.75m, dado.RendaMedia);
    }

    [Fact]
    public void Deve_rejeitar_uf_nula()
    {
        Assert.Throws<ArgumentNullException>(() => new DadoRendaMedia(
            uf: null!,
            trimestre: CriarTrimestre(),
            rendaMedia: 3250.75m));
    }

    [Fact]
    public void Deve_rejeitar_trimestre_nulo()
    {
        Assert.Throws<ArgumentNullException>(() => new DadoRendaMedia(
            uf: CriarUf(),
            trimestre: null!,
            rendaMedia: 3250.75m));
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-10)]
    public void Deve_rejeitar_renda_media_negativa(decimal rendaMedia)
    {
        Assert.Throws<ArgumentException>(() => new DadoRendaMedia(
            uf: CriarUf(),
            trimestre: CriarTrimestre(),
            rendaMedia: rendaMedia));
    }

    [Fact]
    public void Deve_aceitar_renda_media_zero()
    {
        var dado = new DadoRendaMedia(
            uf: CriarUf(),
            trimestre: CriarTrimestre(),
            rendaMedia: 0m);

        Assert.Equal(0m, dado.RendaMedia);
    }
}
