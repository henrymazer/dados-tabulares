using Etl.Pnad;
using DadosTabulares.Domain.Pnad;

namespace Etl.Tests.Pnad;

public sealed class PnadTransformerTests
{
    [Fact]
    public void TransformarDesemprego_Cria_entidades_de_dominio()
    {
        var transformer = new PnadTransformer();
        var rows = new[]
        {
            new PnadCsvRow("sp", 2024, 1, 8.5m),
            new PnadCsvRow("rj", 2024, 1, 7.2m)
        };

        var dados = transformer.TransformarDesemprego(rows);

        Assert.Collection(dados,
            dado =>
            {
                Assert.Equal("SP", dado.UF.Sigla);
                Assert.Equal(new Trimestre(2024, 1), dado.Trimestre);
                Assert.Equal(8.5m, dado.TaxaDesemprego);
            },
            dado =>
            {
                Assert.Equal("RJ", dado.UF.Sigla);
                Assert.Equal(new Trimestre(2024, 1), dado.Trimestre);
                Assert.Equal(7.2m, dado.TaxaDesemprego);
            });
    }
}
