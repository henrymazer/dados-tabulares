using System.Text;
using Data;
using Etl;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Etl.Tests.Ingestion;

public sealed class IngestionSnapshotIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgis/postgis:17-3.5")
        .WithDatabase("dados_publicos")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public async Task InitializeAsync()
        => await _postgres.StartAsync();

    public async Task DisposeAsync()
        => await _postgres.DisposeAsync();

    [Fact]
    public async Task RunAsync_ComMesmoSnapshot_NaoDuplicaCargaNemMetadadosCorrentes()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();

        var csvPath = CreateTempCsv("""
            codigo_ibge,municipio,uf,faixa_etaria,raca,quantidade
            3550308,Sao Paulo,SP,0 a 4 anos,Branca,150000
            """);

        try
        {
            var runner = new EtlRunner(context);
            var command = new EtlCommand(EtlSourceKind.Ibge, "populacional", csvPath);

            var primeiraCarga = await runner.RunAsync(command);
            var segundaCarga = await runner.RunAsync(command);

            Assert.Equal(1, primeiraCarga);
            Assert.Equal(0, segundaCarga);
            Assert.Equal(1, await context.DadosPopulacionais.CountAsync());

            var snapshots = await context.CargasBrutasSnapshots
                .Where(x => x.Fonte == "ibge" && x.Dataset == "populacional")
                .ToListAsync();

            var snapshotAtual = Assert.Single(snapshots);
            Assert.True(snapshotAtual.IsCurrent);
            Assert.Equal(Path.GetFileName(csvPath), snapshotAtual.NomeArquivoOriginal);
            Assert.Equal(Path.GetFileNameWithoutExtension(csvPath), snapshotAtual.ChaveSnapshot);
            Assert.Equal(1, snapshotAtual.RegistrosImportados);
            Assert.False(string.IsNullOrWhiteSpace(snapshotAtual.HashSnapshot));
        }
        finally
        {
            File.Delete(csvPath);
        }
    }

    [Fact]
    public async Task RunAsync_ComNovoSnapshot_SubstituiSnapshotAnteriorDoMesmoDataset()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();

        var primeiroCsv = CreateTempCsv("""
            codigo_ibge,municipio,uf,faixa_etaria,raca,quantidade
            3550308,Sao Paulo,SP,0 a 4 anos,Branca,150000
            """);

        var segundoCsv = CreateTempCsv("""
            codigo_ibge,municipio,uf,faixa_etaria,raca,quantidade
            4106902,Curitiba,PR,0 a 4 anos,Branca,95000
            """);

        try
        {
            var runner = new EtlRunner(context);

            await runner.RunAsync(new EtlCommand(EtlSourceKind.Ibge, "populacional", primeiroCsv));
            var segundaCarga = await runner.RunAsync(new EtlCommand(EtlSourceKind.Ibge, "populacional", segundoCsv));

            Assert.Equal(1, segundaCarga);
            Assert.Equal(1, await context.DadosPopulacionais.CountAsync());

            var row = await context.DadosPopulacionais.SingleAsync();
            Assert.Equal(4106902, row.MunicipioCodigoIbge);
            Assert.Equal("Curitiba", row.MunicipioNome);
            Assert.Equal("PR", row.UfSigla);

            var snapshots = await context.CargasBrutasSnapshots
                .Where(x => x.Fonte == "ibge" && x.Dataset == "populacional")
                .OrderBy(x => x.Id)
                .ToListAsync();

            Assert.Equal(2, snapshots.Count);
            Assert.False(snapshots[0].IsCurrent);
            Assert.NotNull(snapshots[0].SubstituidoEmUtc);
            Assert.True(snapshots[1].IsCurrent);
            Assert.Equal(Path.GetFileName(segundoCsv), snapshots[1].NomeArquivoOriginal);
        }
        finally
        {
            File.Delete(primeiroCsv);
            File.Delete(segundoCsv);
        }
    }

    private PublicDataDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PublicDataDbContext>()
            .UseNpgsql(_postgres.GetConnectionString(), options => options.UseNetTopologySuite())
            .Options;

        return new PublicDataDbContext(options);
    }

    private static string CreateTempCsv(string contents)
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.csv");
        File.WriteAllText(path, contents, Encoding.UTF8);
        return path;
    }
}
