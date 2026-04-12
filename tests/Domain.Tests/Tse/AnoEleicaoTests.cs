namespace Domain.Tests.Tse;

using DadosTabulares.Domain.Tse;

public class AnoEleicaoTests
{
    [Theory]
    [InlineData(2022)]
    [InlineData(2020)]
    [InlineData(2018)]
    [InlineData(2024)]
    public void Deve_criar_ano_eleicao_valido(int ano)
    {
        var anoEleicao = new AnoEleicao(ano);
        Assert.Equal(ano, anoEleicao.Ano);
    }

    [Theory]
    [InlineData(2023)]
    [InlineData(2019)]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_rejeitar_ano_nao_eleitoral(int ano)
    {
        Assert.Throws<ArgumentException>(() => new AnoEleicao(ano));
    }

    [Fact]
    public void Dois_anos_iguais_devem_ser_iguais()
    {
        var a = new AnoEleicao(2022);
        var b = new AnoEleicao(2022);
        Assert.Equal(a, b);
    }

    [Fact]
    public void ToString_deve_retornar_ano()
    {
        var anoEleicao = new AnoEleicao(2022);
        Assert.Equal("2022", anoEleicao.ToString());
    }
}
