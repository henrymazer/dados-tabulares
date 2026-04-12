namespace Domain.Tests.Tse;

using DadosTabulares.Domain.Tse;

public class SecaoEleitoralTests
{
    [Fact]
    public void Deve_criar_secao_eleitoral_valida()
    {
        var municipio = new Municipio(3550308, "São Paulo", new UF("SP"));
        var zona = new ZonaEleitoral(1, municipio);
        var secao = new SecaoEleitoral(42, zona);

        Assert.Equal(42, secao.NumeroSecao);
        Assert.Equal(zona, secao.ZonaEleitoral);
    }

    [Fact]
    public void Deve_rejeitar_numero_secao_invalido()
    {
        var municipio = new Municipio(3550308, "São Paulo", new UF("SP"));
        var zona = new ZonaEleitoral(1, municipio);
        Assert.Throws<ArgumentException>(() => new SecaoEleitoral(0, zona));
    }

    [Fact]
    public void Deve_rejeitar_zona_nula()
    {
        Assert.Throws<ArgumentNullException>(() => new SecaoEleitoral(42, null!));
    }
}
