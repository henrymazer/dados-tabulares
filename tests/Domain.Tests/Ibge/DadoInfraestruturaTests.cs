namespace Domain.Tests.Ibge;

using DadosTabulares.Domain.Ibge;
using DadosTabulares.Domain.Tse;

public class DadoInfraestruturaTests
{
    private static Municipio CriarMunicipio() =>
        new(3550308, "São Paulo", new UF("SP"));

    [Fact]
    public void Deve_criar_dado_infraestrutura_valido()
    {
        var municipio = CriarMunicipio();

        var dado = new DadoInfraestrutura(
            municipio: municipio,
            tipoInfraestrutura: "Energia elétrica",
            domiciliosAtendidos: 450000);

        Assert.Equal(municipio, dado.Municipio);
        Assert.Equal("Energia elétrica", dado.TipoInfraestrutura);
        Assert.Equal(450000, dado.DomiciliosAtendidos);
    }

    [Fact]
    public void Deve_rejeitar_tipo_infraestrutura_vazio()
    {
        Assert.Throws<ArgumentException>(() => new DadoInfraestrutura(
            municipio: CriarMunicipio(), tipoInfraestrutura: "", domiciliosAtendidos: 100));
    }

    [Fact]
    public void Deve_rejeitar_domicilios_negativos()
    {
        Assert.Throws<ArgumentException>(() => new DadoInfraestrutura(
            municipio: CriarMunicipio(), tipoInfraestrutura: "Energia elétrica", domiciliosAtendidos: -1));
    }

    [Fact]
    public void Deve_aceitar_zero_domicilios()
    {
        var dado = new DadoInfraestrutura(
            municipio: CriarMunicipio(), tipoInfraestrutura: "Internet", domiciliosAtendidos: 0);

        Assert.Equal(0, dado.DomiciliosAtendidos);
    }

    [Fact]
    public void Deve_rejeitar_municipio_nulo()
    {
        Assert.Throws<ArgumentNullException>(() => new DadoInfraestrutura(
            municipio: null!, tipoInfraestrutura: "Energia elétrica", domiciliosAtendidos: 100));
    }
}
