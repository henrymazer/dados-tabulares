using System.Text;
using Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Etl.Tests;

public sealed class EtlRunnerSnapshotIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgis/postgis:17-3.5")
        .WithDatabase("dados_publicos")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public async Task InitializeAsync()
        => await _postgresContainer.StartAsync();

    public async Task DisposeAsync()
        => await _postgresContainer.DisposeAsync();

    [Fact]
    public async Task RunAsync_ComMesmoSnapshot_NaoDuplicaCargaNemSnapshot()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();

        var runner = new EtlRunner(context);
        var filePath = CreateSetoresGeoJsonFile("355030800000001");
        var command = new EtlCommand(EtlSourceKind.Ibge, "setores-censitarios", filePath, "setores-censitarios");

        var firstRun = await runner.RunAsync(command);
        var secondRun = await runner.RunAsync(command);

        Assert.Equal(1, firstRun);
        Assert.Equal(0, secondRun);
        Assert.Equal(1, await context.SetoresCensitarios.CountAsync());
        Assert.Equal(1, await context.CargasBrutasSnapshots.CountAsync());

        var snapshot = await context.CargasBrutasSnapshots.SingleAsync();
        Assert.Equal("ibge", snapshot.Fonte);
        Assert.Equal("setores-censitarios", snapshot.Dataset);
        Assert.Equal(Path.GetFullPath(filePath), snapshot.CaminhoArquivoOriginal);
        Assert.Equal(Path.GetFileName(filePath), snapshot.NomeArquivoOriginal);
        Assert.Equal(1, await context.CargasBrutasAuditorias.CountAsync());
    }

    private PublicDataDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PublicDataDbContext>()
            .UseNpgsql(_postgresContainer.GetConnectionString(), options => options.UseNetTopologySuite())
            .Options;

        return new PublicDataDbContext(options);
    }

    private static string CreateSetoresGeoJsonFile(string codigoSetor)
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.geojson");
        File.WriteAllText(filePath, $$"""
        {
          "type": "FeatureCollection",
          "features": [
            {
              "type": "Feature",
              "properties": {
                "cd_setor": "{{codigoSetor}}",
                "cd_mun": 3550308,
                "nm_mun": "Sao Paulo",
                "uf": "SP"
              },
              "geometry": {
                "type": "Polygon",
                "coordinates": [[
                  [-46.65, -23.55],
                  [-46.64, -23.55],
                  [-46.64, -23.54],
                  [-46.65, -23.54],
                  [-46.65, -23.55]
                ]]
              }
            }
          ]
        }
        """, Encoding.UTF8);

        return filePath;
    }
}
