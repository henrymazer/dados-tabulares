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
}
