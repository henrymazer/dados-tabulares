using System.Text;
using Data;
using Etl.Ibge;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Etl.Tests.Ibge;

public sealed class IbgeIngestionPipelineIntegrationTests : IAsyncLifetime
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
    public async Task IngerirAsync_IsolaDatasets_E_NaoDuplicaDados()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();

        var pipeline = new IbgeIngestionPipeline(context);

        var populacional = await pipeline.IngerirAsync(IbgeDataset.Populacional, ToStream("""
            codigo_ibge,municipio,uf,faixa_etaria,raca,quantidade
            3550308,Sao Paulo,SP,0 a 4 anos,Branca,150000
            """));

        Assert.Equal(1, populacional);
        Assert.Equal(1, await context.DadosPopulacionais.CountAsync());
        Assert.Equal(0, await context.DadosRenda.CountAsync());

        var renda = await pipeline.IngerirAsync(IbgeDataset.Renda, ToStream("""
            codigo_ibge,municipio,uf,faixa_renda,quantidade
            3550308,Sao Paulo,SP,1 a 2 salarios minimos,85000
            """));

        Assert.Equal(1, renda);
        Assert.Equal(1, await context.DadosPopulacionais.CountAsync());
        Assert.Equal(1, await context.DadosRenda.CountAsync());

        var repasse = await pipeline.IngerirAsync(IbgeDataset.Populacional, ToStream("""
            codigo_ibge,municipio,uf,faixa_etaria,raca,quantidade
            3550308,Sao Paulo,SP,0 a 4 anos,Branca,150000
            """));

        Assert.Equal(1, repasse);
        Assert.Equal(1, await context.DadosPopulacionais.CountAsync());

        var row = await context.DadosPopulacionais.SingleAsync();
        Assert.Equal(3550308, row.MunicipioCodigoIbge);
        Assert.Equal("Sao Paulo", row.MunicipioNome);
        Assert.Equal("SP", row.UfSigla);
        Assert.Equal("0 a 4 anos", row.FaixaEtaria);
        Assert.Equal("Branca", row.Raca);
        Assert.Equal(150000, row.Quantidade);
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
