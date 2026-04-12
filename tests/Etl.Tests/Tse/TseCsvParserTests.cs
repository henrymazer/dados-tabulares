using System.Globalization;
using System.IO;
using DadosTabulares.Domain.Tse;
using DadosTabulares.Etl.Tse;

namespace Etl.Tests.Tse;

public class TseCsvParserTests
{
    [Fact]
    public async Task ParseResultadosAsync_TransformsCsvIntoDomainEntities()
    {
        var csv = """
                  NR_CPF_CANDIDATO;NM_CANDIDATO;NM_URNA_CANDIDATO;NR_CANDIDATO;DS_CARGO;NR_PARTIDO;SG_PARTIDO;NM_PARTIDO;SG_UF;CD_MUNICIPIO;NM_MUNICIPIO;NR_ZONA;NR_SECAO;QT_VOTOS
                  12345678900;Maria Silva;Maria;13;Prefeita;13;PT;Partido dos Trabalhadores;SP;3550308;Sao Paulo;1;101;1234
                  """;

        var parser = new TseCsvParser();

        var resultados = await parser.ParseResultadosAsync(new StringReader(csv), new AnoEleicao(2024), CultureInfo.InvariantCulture);

        var resultado = Assert.Single(resultados);
        Assert.Equal("12345678900", resultado.Candidato.Cpf);
        Assert.Equal("Maria Silva", resultado.Candidato.Nome);
        Assert.Equal("Maria", resultado.Candidato.NomeUrna);
        Assert.Equal(13, resultado.Candidato.Numero);
        Assert.Equal("Prefeita", resultado.Candidato.Cargo);
        Assert.Equal("PT", resultado.Candidato.Partido.Sigla);
        Assert.Equal(2024, resultado.AnoEleicao.Ano);
        Assert.Equal(1, resultado.Turno);
        Assert.Equal(3550308, resultado.ZonaEleitoral.Municipio.CodigoIbge);
        Assert.Equal(101, resultado.SecaoEleitoral.NumeroSecao);
        Assert.Equal(1234, resultado.QuantidadeVotos);
    }

    [Fact]
    public async Task ParseResultadosAsync_RejectsUnexpectedNumericFormat()
    {
        var csv = """
                  NR_CPF_CANDIDATO;NM_CANDIDATO;NM_URNA_CANDIDATO;NR_CANDIDATO;DS_CARGO;NR_PARTIDO;SG_PARTIDO;NM_PARTIDO;SG_UF;CD_MUNICIPIO;NM_MUNICIPIO;NR_ZONA;NR_SECAO;QT_VOTOS
                  12345678900;Maria Silva;Maria;13;Prefeita;13;PT;Partido dos Trabalhadores;SP;3550308;Sao Paulo;1;101;12x
                  """;

        var parser = new TseCsvParser();

        await Assert.ThrowsAsync<FormatException>(() => parser.ParseResultadosAsync(new StringReader(csv), new AnoEleicao(2024)));
    }

    [Fact]
    public async Task ParseColigacoesAsync_GroupsRowsByCoalitionName()
    {
        var csv = """
                  NM_COLIGACAO;NR_PARTIDO;SG_PARTIDO;NM_PARTIDO
                  Brasil da Esperança;13;PT;Partido dos Trabalhadores
                  Brasil da Esperança;65;PCdoB;Partido Comunista do Brasil
                  """;

        var parser = new TseCsvParser();

        var coligacoes = await parser.ParseColigacoesAsync(new StringReader(csv), new AnoEleicao(2024));

        var coligacao = Assert.Single(coligacoes);
        Assert.Equal("Brasil da Esperança", coligacao.Nome);
        Assert.Equal(2, coligacao.Partidos.Count);
        Assert.Equal("PT", coligacao.Partidos[0].Sigla);
        Assert.Equal("PCdoB", coligacao.Partidos[1].Sigla);
    }
}
