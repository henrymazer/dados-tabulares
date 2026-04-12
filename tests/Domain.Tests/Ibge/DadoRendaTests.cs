namespace Domain.Tests.Ibge;

using DadosTabulares.Domain.Ibge;
using DadosTabulares.Domain.Tse;

public class DadoRendaTests
{
    private static Municipio CriarMunicipio() =>
        new(3550308, "São Paulo", new UF("SP"));

    [Fact]
    public void Deve_criar_dado_renda_valido()
    {
        var municipio = CriarMunicipio();

        var dado = new DadoRenda(
            municipio: municipio,
            faixaRenda: "1 a 2 salários mínimos",
            quantidade: 85000);

        Assert.Equal(municipio, dado.Municipio);
        Assert.Equal("1 a 2 salários mínimos", dado.FaixaRenda);
        Assert.Equal(85000, dado.Quantidade);
    }

    [Fact]
    public void Deve_rejeitar_faixa_renda_vazia()
    {
        Assert.Throws<ArgumentException>(() => new DadoRenda(
            municipio: CriarMunicipio(), faixaRenda: "", quantidade: 100));
    }

    [Fact]
    public void Deve_rejeitar_quantidade_negativa()
    {
        Assert.Throws<ArgumentException>(() => new DadoRenda(
            municipio: CriarMunicipio(), faixaRenda: "1 a 2 salários mínimos", quantidade: -1));
    }

    [Fact]
    public void Deve_aceitar_quantidade_zero()
    {
        var dado = new DadoRenda(
            municipio: CriarMunicipio(), faixaRenda: "1 a 2 salários mínimos", quantidade: 0);

        Assert.Equal(0, dado.Quantidade);
    }

    [Fact]
    public void Deve_rejeitar_municipio_nulo()
    {
        Assert.Throws<ArgumentNullException>(() => new DadoRenda(
            municipio: null!, faixaRenda: "1 a 2 salários mínimos", quantidade: 100));
    }
}
