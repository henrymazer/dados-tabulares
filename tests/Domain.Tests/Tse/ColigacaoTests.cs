namespace Domain.Tests.Tse;

using DadosTabulares.Domain.Tse;

public class ColigacaoTests
{
    [Fact]
    public void Deve_criar_coligacao_valida()
    {
        var pt = new Partido(13, "PT", "Partido dos Trabalhadores");
        var pcdob = new Partido(65, "PCdoB", "Partido Comunista do Brasil");
        var coligacao = new Coligacao("Brasil da Esperança", new AnoEleicao(2022), [pt, pcdob]);

        Assert.Equal("Brasil da Esperança", coligacao.Nome);
        Assert.Equal(2022, coligacao.AnoEleicao.Ano);
        Assert.Equal(2, coligacao.Partidos.Count);
    }

    [Fact]
    public void Deve_rejeitar_nome_vazio()
    {
        var pt = new Partido(13, "PT", "Partido dos Trabalhadores");
        Assert.Throws<ArgumentException>(() => new Coligacao("", new AnoEleicao(2022), [pt]));
    }

    [Fact]
    public void Deve_rejeitar_coligacao_sem_partidos()
    {
        Assert.Throws<ArgumentException>(() => new Coligacao("Coligação X", new AnoEleicao(2022), []));
    }
}
