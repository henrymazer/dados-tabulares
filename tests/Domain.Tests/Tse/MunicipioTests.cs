namespace Domain.Tests.Tse;

using DadosTabulares.Domain.Tse;

public class MunicipioTests
{
    [Fact]
    public void Deve_criar_municipio_valido()
    {
        var municipio = new Municipio(3550308, "São Paulo", new UF("SP"));
        Assert.Equal(3550308, municipio.CodigoIbge);
        Assert.Equal("São Paulo", municipio.Nome);
        Assert.Equal("SP", municipio.UF.Sigla);
    }

    [Fact]
    public void Deve_rejeitar_nome_vazio()
    {
        Assert.Throws<ArgumentException>(() => new Municipio(3550308, "", new UF("SP")));
    }

    [Fact]
    public void Deve_rejeitar_codigo_ibge_invalido()
    {
        Assert.Throws<ArgumentException>(() => new Municipio(0, "São Paulo", new UF("SP")));
    }

    [Fact]
    public void Dois_municipios_iguais_devem_ser_iguais()
    {
        var a = new Municipio(3550308, "São Paulo", new UF("SP"));
        var b = new Municipio(3550308, "São Paulo", new UF("SP"));
        Assert.Equal(a, b);
    }

    [Fact]
    public void ToString_deve_retornar_nome_e_uf()
    {
        var municipio = new Municipio(3550308, "São Paulo", new UF("SP"));
        Assert.Equal("São Paulo/SP", municipio.ToString());
    }
}
