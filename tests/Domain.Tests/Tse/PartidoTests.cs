namespace Domain.Tests.Tse;

using DadosTabulares.Domain.Tse;

public class PartidoTests
{
    [Fact]
    public void Deve_criar_partido_valido()
    {
        var partido = new Partido(13, "PT", "Partido dos Trabalhadores");
        Assert.Equal(13, partido.Numero);
        Assert.Equal("PT", partido.Sigla);
        Assert.Equal("Partido dos Trabalhadores", partido.Nome);
    }

    [Fact]
    public void Deve_rejeitar_numero_invalido()
    {
        Assert.Throws<ArgumentException>(() => new Partido(0, "PT", "Partido dos Trabalhadores"));
    }

    [Fact]
    public void Deve_rejeitar_sigla_vazia()
    {
        Assert.Throws<ArgumentException>(() => new Partido(13, "", "Partido dos Trabalhadores"));
    }

    [Fact]
    public void Deve_rejeitar_nome_vazio()
    {
        Assert.Throws<ArgumentException>(() => new Partido(13, "PT", ""));
    }
}
