namespace Domain.Tests.Tse;

using DadosTabulares.Domain.Tse;

public class ResultadoEleitoralTests
{
    private static Candidato CriarCandidato() => new(
        cpf: "12345678901", nome: "João", nomeUrna: "João",
        numero: 1313, cargo: "Vereador",
        partido: new Partido(13, "PT", "Partido dos Trabalhadores"),
        anoEleicao: new AnoEleicao(2022), uf: new UF("SP"));

    private static ZonaEleitoral CriarZona() =>
        new(1, new Municipio(3550308, "São Paulo", new UF("SP")));

    [Fact]
    public void Deve_criar_resultado_eleitoral_valido()
    {
        var candidato = CriarCandidato();
        var zona = CriarZona();
        var secao = new SecaoEleitoral(42, zona);

        var resultado = new ResultadoEleitoral(
            candidato: candidato,
            anoEleicao: new AnoEleicao(2022),
            turno: 1,
            zonaEleitoral: zona,
            secaoEleitoral: secao,
            quantidadeVotos: 1500);

        Assert.Equal(candidato, resultado.Candidato);
        Assert.Equal(2022, resultado.AnoEleicao.Ano);
        Assert.Equal(1, resultado.Turno);
        Assert.Equal(zona, resultado.ZonaEleitoral);
        Assert.Equal(secao, resultado.SecaoEleitoral);
        Assert.Equal(1500, resultado.QuantidadeVotos);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(-1)]
    public void Deve_rejeitar_turno_invalido(int turno)
    {
        Assert.Throws<ArgumentException>(() => new ResultadoEleitoral(
            candidato: CriarCandidato(), anoEleicao: new AnoEleicao(2022),
            turno: turno, zonaEleitoral: CriarZona(),
            secaoEleitoral: new SecaoEleitoral(42, CriarZona()),
            quantidadeVotos: 100));
    }

    [Fact]
    public void Deve_rejeitar_votos_negativos()
    {
        Assert.Throws<ArgumentException>(() => new ResultadoEleitoral(
            candidato: CriarCandidato(), anoEleicao: new AnoEleicao(2022),
            turno: 1, zonaEleitoral: CriarZona(),
            secaoEleitoral: new SecaoEleitoral(42, CriarZona()),
            quantidadeVotos: -1));
    }

    [Fact]
    public void Deve_aceitar_zero_votos()
    {
        var resultado = new ResultadoEleitoral(
            candidato: CriarCandidato(), anoEleicao: new AnoEleicao(2022),
            turno: 1, zonaEleitoral: CriarZona(),
            secaoEleitoral: new SecaoEleitoral(42, CriarZona()),
            quantidadeVotos: 0);

        Assert.Equal(0, resultado.QuantidadeVotos);
    }
}
