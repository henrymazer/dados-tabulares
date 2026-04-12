namespace Domain.Tests.Ibge;

using DadosTabulares.Domain.Ibge;
using DadosTabulares.Domain.Tse;

public class DadoUrbanizacaoTests
{
    private static Municipio CriarMunicipio() =>
        new(3550308, "São Paulo", new UF("SP"));

    [Fact]
    public void Deve_criar_dado_urbanizacao_valido()
    {
        var municipio = CriarMunicipio();

        var dado = new DadoUrbanizacao(
            municipio: municipio,
            tipoArea: "Urbana",
            populacao: 11200000);

        Assert.Equal(municipio, dado.Municipio);
        Assert.Equal("Urbana", dado.TipoArea);
        Assert.Equal(11200000, dado.Populacao);
    }

    [Fact]
    public void Deve_rejeitar_tipo_area_vazio()
    {
        Assert.Throws<ArgumentException>(() => new DadoUrbanizacao(
            municipio: CriarMunicipio(), tipoArea: "", populacao: 100));
    }

    [Fact]
    public void Deve_rejeitar_populacao_negativa()
    {
        Assert.Throws<ArgumentException>(() => new DadoUrbanizacao(
            municipio: CriarMunicipio(), tipoArea: "Urbana", populacao: -1));
    }

    [Fact]
    public void Deve_aceitar_populacao_zero()
    {
        var dado = new DadoUrbanizacao(
            municipio: CriarMunicipio(), tipoArea: "Rural", populacao: 0);

        Assert.Equal(0, dado.Populacao);
    }

    [Fact]
    public void Deve_rejeitar_municipio_nulo()
    {
        Assert.Throws<ArgumentNullException>(() => new DadoUrbanizacao(
            municipio: null!, tipoArea: "Urbana", populacao: 100));
    }
}
