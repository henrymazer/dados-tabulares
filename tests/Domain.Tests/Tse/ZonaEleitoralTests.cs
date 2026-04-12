namespace Domain.Tests.Tse;

using DadosTabulares.Domain.Tse;

public class ZonaEleitoralTests
{
    [Fact]
    public void Deve_criar_zona_eleitoral_valida()
    {
        var municipio = new Municipio(3550308, "São Paulo", new UF("SP"));
        var zona = new ZonaEleitoral(1, municipio);

        Assert.Equal(1, zona.NumeroZona);
        Assert.Equal(municipio, zona.Municipio);
    }

    [Fact]
    public void Deve_rejeitar_numero_zona_invalido()
    {
        var municipio = new Municipio(3550308, "São Paulo", new UF("SP"));
        Assert.Throws<ArgumentException>(() => new ZonaEleitoral(0, municipio));
    }

    [Fact]
    public void Deve_rejeitar_municipio_nulo()
    {
        Assert.Throws<ArgumentNullException>(() => new ZonaEleitoral(1, null!));
    }
}
