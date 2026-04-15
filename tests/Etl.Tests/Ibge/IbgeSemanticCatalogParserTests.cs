using Etl.Ibge;

namespace Etl.Tests.Ibge;

public sealed class IbgeSemanticCatalogParserTests
{
    [Fact]
    public void ParseDictionary_ComPacoteGeral_PreservaTemaTipoEDescricao()
    {
        var parser = new IbgeSemanticCatalogParser();
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../.."));

        using var stream = File.OpenRead(Path.Combine(repoRoot, "dicionario_de_dados_agregados_por_setores_censitarios_geral.csv"));

        var catalogo = parser.ParseDictionary(stream, "dicionario_de_dados_agregados_por_setores_censitarios_geral.csv");

        var entrada = catalogo.Entries.Single(item =>
            item.Variavel == "V00001" &&
            item.Tema == "Características do Domicílio - Parte 1");

        Assert.Equal("geral", entrada.Pacote);
        Assert.Equal("Domicílio", entrada.Tipo);
        Assert.Equal("Domicílios Particulares Permanentes Ocupados", entrada.Descricao);
    }
}
