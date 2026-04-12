namespace Domain.Tests.Ibge;

using DadosTabulares.Domain.Ibge;
using DadosTabulares.Domain.Tse;

public class DadoEscolaridadeTests
{
    private static Municipio CriarMunicipio() =>
        new(3550308, "São Paulo", new UF("SP"));

    [Fact]
    public void Deve_criar_dado_escolaridade_valido()
    {
        var municipio = CriarMunicipio();

        var dado = new DadoEscolaridade(
            municipio: municipio,
            nivelEscolaridade: "Ensino Médio Completo",
            quantidade: 200000);

        Assert.Equal(municipio, dado.Municipio);
        Assert.Equal("Ensino Médio Completo", dado.NivelEscolaridade);
        Assert.Equal(200000, dado.Quantidade);
    }

    [Fact]
    public void Deve_rejeitar_nivel_escolaridade_vazio()
    {
        Assert.Throws<ArgumentException>(() => new DadoEscolaridade(
            municipio: CriarMunicipio(), nivelEscolaridade: "", quantidade: 100));
    }

    [Fact]
    public void Deve_rejeitar_quantidade_negativa()
    {
        Assert.Throws<ArgumentException>(() => new DadoEscolaridade(
            municipio: CriarMunicipio(), nivelEscolaridade: "Ensino Médio Completo", quantidade: -1));
    }

    [Fact]
    public void Deve_aceitar_quantidade_zero()
    {
        var dado = new DadoEscolaridade(
            municipio: CriarMunicipio(), nivelEscolaridade: "Ensino Médio Completo", quantidade: 0);

        Assert.Equal(0, dado.Quantidade);
    }

    [Fact]
    public void Deve_rejeitar_municipio_nulo()
    {
        Assert.Throws<ArgumentNullException>(() => new DadoEscolaridade(
            municipio: null!, nivelEscolaridade: "Ensino Médio Completo", quantidade: 100));
    }
}
