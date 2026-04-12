namespace Domain.Tests.Tse;

using DadosTabulares.Domain.Tse;

public class UFTests
{
    [Theory]
    [InlineData("SP")]
    [InlineData("RJ")]
    [InlineData("MG")]
    [InlineData("AC")]
    [InlineData("DF")]
    public void Deve_criar_uf_valida(string sigla)
    {
        var uf = new UF(sigla);
        Assert.Equal(sigla, uf.Sigla);
    }

    [Theory]
    [InlineData("XX")]
    [InlineData("")]
    [InlineData("ABC")]
    public void Deve_rejeitar_uf_invalida(string sigla)
    {
        Assert.Throws<ArgumentException>(() => new UF(sigla));
    }

    [Fact]
    public void Deve_rejeitar_uf_nula()
    {
        Assert.Throws<ArgumentException>(() => new UF(null!));
    }

    [Fact]
    public void Deve_normalizar_para_maiusculo()
    {
        var uf = new UF("sp");
        Assert.Equal("SP", uf.Sigla);
    }

    [Fact]
    public void Duas_ufs_iguais_devem_ser_iguais()
    {
        var a = new UF("SP");
        var b = new UF("SP");
        Assert.Equal(a, b);
    }

    [Fact]
    public void ToString_deve_retornar_sigla()
    {
        var uf = new UF("SP");
        Assert.Equal("SP", uf.ToString());
    }
}
