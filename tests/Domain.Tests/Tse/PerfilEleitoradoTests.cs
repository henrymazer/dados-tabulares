namespace Domain.Tests.Tse;

using DadosTabulares.Domain.Tse;

public class PerfilEleitoradoTests
{
    private static ZonaEleitoral CriarZona() =>
        new(1, new Municipio(3550308, "São Paulo", new UF("SP")));

    [Fact]
    public void Deve_criar_perfil_eleitorado_valido()
    {
        var zona = CriarZona();
        var perfil = new PerfilEleitorado(
            anoEleicao: new AnoEleicao(2022),
            zonaEleitoral: zona,
            faixaEtaria: "25-34",
            escolaridade: "Superior completo",
            genero: "Feminino",
            quantidadeEleitores: 5000);

        Assert.Equal(2022, perfil.AnoEleicao.Ano);
        Assert.Equal(zona, perfil.ZonaEleitoral);
        Assert.Equal("25-34", perfil.FaixaEtaria);
        Assert.Equal("Superior completo", perfil.Escolaridade);
        Assert.Equal("Feminino", perfil.Genero);
        Assert.Equal(5000, perfil.QuantidadeEleitores);
    }

    [Fact]
    public void Deve_rejeitar_faixa_etaria_vazia()
    {
        Assert.Throws<ArgumentException>(() => new PerfilEleitorado(
            anoEleicao: new AnoEleicao(2022), zonaEleitoral: CriarZona(),
            faixaEtaria: "", escolaridade: "Superior", genero: "Masculino",
            quantidadeEleitores: 100));
    }

    [Fact]
    public void Deve_rejeitar_escolaridade_vazia()
    {
        Assert.Throws<ArgumentException>(() => new PerfilEleitorado(
            anoEleicao: new AnoEleicao(2022), zonaEleitoral: CriarZona(),
            faixaEtaria: "25-34", escolaridade: "", genero: "Masculino",
            quantidadeEleitores: 100));
    }

    [Fact]
    public void Deve_rejeitar_genero_vazio()
    {
        Assert.Throws<ArgumentException>(() => new PerfilEleitorado(
            anoEleicao: new AnoEleicao(2022), zonaEleitoral: CriarZona(),
            faixaEtaria: "25-34", escolaridade: "Superior", genero: "",
            quantidadeEleitores: 100));
    }

    [Fact]
    public void Deve_rejeitar_quantidade_negativa()
    {
        Assert.Throws<ArgumentException>(() => new PerfilEleitorado(
            anoEleicao: new AnoEleicao(2022), zonaEleitoral: CriarZona(),
            faixaEtaria: "25-34", escolaridade: "Superior", genero: "Masculino",
            quantidadeEleitores: -1));
    }
}
