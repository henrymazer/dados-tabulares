namespace Domain.Tests.Ibge;

using DadosTabulares.Domain.Ibge;
using DadosTabulares.Domain.Tse;

public class DadoPopulacionalTests
{
    private static Municipio CriarMunicipio() =>
        new(3550308, "São Paulo", new UF("SP"));

    [Fact]
    public void Deve_criar_dado_populacional_valido()
    {
        var municipio = CriarMunicipio();

        var dado = new DadoPopulacional(
            municipio: municipio,
            faixaEtaria: "0 a 4 anos",
            raca: "Branca",
            quantidade: 150000);

        Assert.Equal(municipio, dado.Municipio);
        Assert.Equal("0 a 4 anos", dado.FaixaEtaria);
        Assert.Equal("Branca", dado.Raca);
        Assert.Equal(150000, dado.Quantidade);
    }

    [Fact]
    public void Deve_rejeitar_faixa_etaria_vazia()
    {
        Assert.Throws<ArgumentException>(() => new DadoPopulacional(
            municipio: CriarMunicipio(), faixaEtaria: "", raca: "Branca", quantidade: 100));
    }

    [Fact]
    public void Deve_rejeitar_raca_vazia()
    {
        Assert.Throws<ArgumentException>(() => new DadoPopulacional(
            municipio: CriarMunicipio(), faixaEtaria: "0 a 4 anos", raca: "", quantidade: 100));
    }

    [Fact]
    public void Deve_rejeitar_quantidade_negativa()
    {
        Assert.Throws<ArgumentException>(() => new DadoPopulacional(
            municipio: CriarMunicipio(), faixaEtaria: "0 a 4 anos", raca: "Branca", quantidade: -1));
    }

    [Fact]
    public void Deve_aceitar_quantidade_zero()
    {
        var dado = new DadoPopulacional(
            municipio: CriarMunicipio(), faixaEtaria: "0 a 4 anos", raca: "Branca", quantidade: 0);

        Assert.Equal(0, dado.Quantidade);
    }

    [Fact]
    public void Deve_rejeitar_municipio_nulo()
    {
        Assert.Throws<ArgumentNullException>(() => new DadoPopulacional(
            municipio: null!, faixaEtaria: "0 a 4 anos", raca: "Branca", quantidade: 100));
    }
}
