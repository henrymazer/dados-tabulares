using System.Text;
using Data;
using Etl.Pnad;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Etl.Tests.Pnad;

public sealed class PnadIngestionPipelineIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .WithDatabase("dados_publicos")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public async Task InitializeAsync()
        => await _postgres.StartAsync();

    public async Task DisposeAsync()
        => await _postgres.DisposeAsync();

    [Fact]
    public async Task IngerirAsync_ExecutaCadaDataset_Independente_E_SemDuplicar()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();

        var pipeline = new PnadIngestionPipeline(context);

        var desemprego = await pipeline.IngerirAsync(PnadDataset.Desemprego, ToStream("""
            uf;ano;trimestre;valor
            sp;2024;1;8.5
            """));

        Assert.Equal(1, desemprego);
        Assert.Equal(1, await context.DadosDesemprego.CountAsync());
        Assert.Equal(0, await context.DadosInformalidade.CountAsync());
        Assert.Equal(0, await context.DadosRendaMedia.CountAsync());
        Assert.Equal(1, await context.Trimestres.CountAsync());

        var informalidade = await pipeline.IngerirAsync(PnadDataset.Informalidade, ToStream("""
            uf;ano;trimestre;valor
            sp;2024;1;37.25
            """));

        Assert.Equal(1, informalidade);
        Assert.Equal(1, await context.DadosDesemprego.CountAsync());
        Assert.Equal(1, await context.DadosInformalidade.CountAsync());
        Assert.Equal(0, await context.DadosRendaMedia.CountAsync());
        Assert.Equal(1, await context.Trimestres.CountAsync());

        var rendaMedia = await pipeline.IngerirAsync(PnadDataset.RendaMedia, ToStream("""
            uf;ano;trimestre;valor
            sp;2024;1;3250.75
            """));

        Assert.Equal(1, rendaMedia);
        Assert.Equal(1, await context.DadosDesemprego.CountAsync());
        Assert.Equal(1, await context.DadosInformalidade.CountAsync());
        Assert.Equal(1, await context.DadosRendaMedia.CountAsync());
        Assert.Equal(1, await context.Trimestres.CountAsync());
    }

    [Fact]
    public async Task IngerirAsync_Reexecutado_SobrescreveSemDuplicar()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();

        var pipeline = new PnadIngestionPipeline(context);

        await pipeline.IngerirAsync(PnadDataset.Desemprego, ToStream("""
            uf;ano;trimestre;valor
            sp;2024;1;8.5
            """));

        await pipeline.IngerirAsync(PnadDataset.Desemprego, ToStream("""
            uf;ano;trimestre;valor
            sp;2024;1;7.9
            """));

        var row = await context.DadosDesemprego.SingleAsync();

        Assert.Equal(1, await context.DadosDesemprego.CountAsync());
        Assert.Equal("SP", row.UfSigla);
        Assert.Equal(2024, row.TrimestreAno);
        Assert.Equal(1, row.TrimestreNumero);
        Assert.Equal(7.9m, row.TaxaDesemprego);
        Assert.Equal(1, await context.Trimestres.CountAsync());
    }

    private PublicDataDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PublicDataDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        return new PublicDataDbContext(options);
    }

    private static MemoryStream ToStream(string csv)
        => new(Encoding.UTF8.GetBytes(csv));
}
