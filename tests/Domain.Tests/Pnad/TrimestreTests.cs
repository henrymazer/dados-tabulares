namespace Domain.Tests.Pnad;

using DadosTabulares.Domain.Pnad;

public class TrimestreTests
{
    [Fact]
    public void Deve_criar_trimestre_valido()
    {
        var trimestre = new Trimestre(2024, 1);

        Assert.Equal(2024, trimestre.Ano);
        Assert.Equal(1, trimestre.Numero);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_rejeitar_ano_invalido(int ano)
    {
        Assert.Throws<ArgumentException>(() => new Trimestre(ano, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(-1)]
    public void Deve_rejeitar_numero_de_trimestre_invalido(int numero)
    {
        Assert.Throws<ArgumentException>(() => new Trimestre(2024, numero));
    }
}
