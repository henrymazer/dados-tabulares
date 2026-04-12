namespace Domain.Tests.Ibge;

using DadosTabulares.Domain.Ibge;
using DadosTabulares.Domain.Tse;

public class DadoSaneamentoTests
{
    private static Municipio CriarMunicipio() =>
        new(3550308, "São Paulo", new UF("SP"));

    [Fact]
    public void Deve_criar_dado_saneamento_valido()
    {
        var municipio = CriarMunicipio();

        var dado = new DadoSaneamento(
            municipio: municipio,
            tipoSaneamento: "Rede geral de esgoto",
            domiciliosAtendidos: 320000);

        Assert.Equal(municipio, dado.Municipio);
        Assert.Equal("Rede geral de esgoto", dado.TipoSaneamento);
        Assert.Equal(320000, dado.DomiciliosAtendidos);
    }

    [Fact]
    public void Deve_rejeitar_tipo_saneamento_vazio()
    {
        Assert.Throws<ArgumentException>(() => new DadoSaneamento(
            municipio: CriarMunicipio(), tipoSaneamento: "", domiciliosAtendidos: 100));
    }

    [Fact]
    public void Deve_rejeitar_domicilios_negativos()
    {
        Assert.Throws<ArgumentException>(() => new DadoSaneamento(
            municipio: CriarMunicipio(), tipoSaneamento: "Rede geral de esgoto", domiciliosAtendidos: -1));
    }

    [Fact]
    public void Deve_aceitar_zero_domicilios()
    {
        var dado = new DadoSaneamento(
            municipio: CriarMunicipio(), tipoSaneamento: "Rede geral de esgoto", domiciliosAtendidos: 0);

        Assert.Equal(0, dado.DomiciliosAtendidos);
    }

    [Fact]
    public void Deve_rejeitar_municipio_nulo()
    {
        Assert.Throws<ArgumentNullException>(() => new DadoSaneamento(
            municipio: null!, tipoSaneamento: "Rede geral de esgoto", domiciliosAtendidos: 100));
    }
}
