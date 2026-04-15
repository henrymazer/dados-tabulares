namespace Etl.Tests;

public sealed class EtlCommandParserTests
{
    [Fact]
    public void Parse_ComSourceTseDireto_ResolveAnoEArquivo()
    {
        var command = EtlCommandParser.Parse([
            "--source", "prestacao-contas",
            "--file", "/tmp/prestacoes.csv",
            "--year", "2024"
        ]);

        Assert.Equal(EtlSourceKind.Tse, command.Kind);
        Assert.Equal("prestacao-contas", command.Source);
        Assert.Equal("/tmp/prestacoes.csv", command.FilePath);
        Assert.Equal(2024, command.Year);
    }

    [Fact]
    public void Parse_ComSourceAgrupadoIbge_ExigeDataset()
    {
        var command = EtlCommandParser.Parse([
            "--source", "ibge",
            "--dataset", "urbanizacao",
            "--file", "/tmp/urbanizacao.csv"
        ]);

        Assert.Equal(EtlSourceKind.Ibge, command.Kind);
        Assert.Equal("urbanizacao", command.Source);
        Assert.Equal("/tmp/urbanizacao.csv", command.FilePath);
    }

    [Fact]
    public void Parse_ComDatasetEspacialIbge_ResolveArquivo()
    {
        var command = EtlCommandParser.Parse([
            "--source", "ibge",
            "--dataset", "setores-censitarios",
            "--file", "/tmp/setores.geojson"
        ]);

        Assert.Equal(EtlSourceKind.Ibge, command.Kind);
        Assert.Equal("setores-censitarios", command.Source);
        Assert.Equal("/tmp/setores.geojson", command.FilePath);
    }

    [Fact]
    public void Parse_ComMalhaMunicipalIbge_ResolveArquivo()
    {
        var command = EtlCommandParser.Parse([
            "--source", "ibge",
            "--dataset", "malha-municipal",
            "--file", "/tmp/municipios_2025.gpkg"
        ]);

        Assert.Equal(EtlSourceKind.Ibge, command.Kind);
        Assert.Equal("malha-municipal", command.Source);
        Assert.Equal("/tmp/municipios_2025.gpkg", command.FilePath);
    }

    [Fact]
    public void Parse_ComAgregadosStagingIbge_UsaNomeDoArquivoComoDatasetDoSnapshot()
    {
        var command = EtlCommandParser.Parse([
            "--source", "ibge",
            "--dataset", "agregados-staging",
            "--file", "/tmp/Agregados_por_setores_basico_BR_20250417.zip"
        ]);

        Assert.Equal("agregados-staging", command.Source);
        Assert.Equal("agregados_por_setores_basico_br_20250417", command.Dataset);
    }

    [Fact]
    public void Parse_ComSourceAgrupadoPnad_ExigeDataset()
    {
        var command = EtlCommandParser.Parse([
            "--source", "pnad",
            "--dataset", "renda-media",
            "--file", "/tmp/pnad.csv"
        ]);

        Assert.Equal(EtlSourceKind.Pnad, command.Kind);
        Assert.Equal("renda-media", command.Source);
        Assert.Equal("/tmp/pnad.csv", command.FilePath);
    }

    [Fact]
    public void Parse_TseSemAno_Falha()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => EtlCommandParser.Parse([
            "--source", "resultados",
            "--file", "/tmp/resultados.csv"
        ]));

        Assert.Contains("--year", ex.Message);
    }

    [Fact]
    public void Parse_ComLocaisBrutosTse_UsaNomeDoArquivoComoDatasetDoSnapshot()
    {
        var command = EtlCommandParser.Parse([
            "--source", "tse",
            "--dataset", "locais-brutos",
            "--file", "/tmp/detalhe_votacao_secao_2024.zip",
            "--year", "2024"
        ]);

        Assert.Equal(EtlSourceKind.Tse, command.Kind);
        Assert.Equal("locais-brutos", command.Source);
        Assert.Equal("detalhe_votacao_secao_2024", command.Dataset);
        Assert.Equal(2024, command.Year);
    }
}
