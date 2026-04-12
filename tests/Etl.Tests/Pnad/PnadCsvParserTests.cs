using Etl.Pnad;

namespace Etl.Tests.Pnad;

public sealed class PnadCsvParserTests
{
    [Fact]
    public void Parse_Converte_csv_em_linhas_brutas_normalizadas()
    {
        const string csv = """
                           uf;ano;trimestre;valor
                           sp;2024;1;8,5
                           rj;2024;2;9.25
                           """;

        var parser = new PnadCsvParser();

        var rows = parser.Parse(new StringReader(csv));

        Assert.Collection(rows,
            row =>
            {
                Assert.Equal("SP", row.UfSigla);
                Assert.Equal(2024, row.Ano);
                Assert.Equal(1, row.Trimestre);
                Assert.Equal(8.5m, row.Valor);
            },
            row =>
            {
                Assert.Equal("RJ", row.UfSigla);
                Assert.Equal(2024, row.Ano);
                Assert.Equal(2, row.Trimestre);
                Assert.Equal(9.25m, row.Valor);
            });
    }
}
