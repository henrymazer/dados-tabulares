using System.Text;
using Data;
using Etl.Ibge;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Etl.Tests.Ibge;

public sealed class IbgeIngestionPipelineIntegrationTests : IAsyncLifetime
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

    [Fact]
    public async Task IngerirSetoresCensitariosAsync_SubstituiCargaEspacial_E_PersisteMultiPolygon()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();

        var pipeline = new IbgeSpatialIngestionPipeline(context);

        var primeiroLote = await pipeline.IngerirSetoresCensitariosAsync(ToStream("""
            {
              "type": "FeatureCollection",
              "features": [
                {
                  "type": "Feature",
                  "properties": {
                    "cd_setor": "355030800000001",
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
            """));

        Assert.Equal(1, primeiroLote);
        Assert.Equal(1, await context.SetoresCensitarios.CountAsync());

        var repasse = await pipeline.IngerirSetoresCensitariosAsync(ToStream("""
            {
              "type": "FeatureCollection",
              "features": [
                {
                  "type": "Feature",
                  "properties": {
                    "cd_setor": "355030800000002",
                    "cd_mun": 3550308,
                    "nm_mun": "Sao Paulo",
                    "uf": "SP"
                  },
                  "geometry": {
                    "type": "MultiPolygon",
                    "coordinates": [[[
                      [-46.63, -23.55],
                      [-46.62, -23.55],
                      [-46.62, -23.54],
                      [-46.63, -23.54],
                      [-46.63, -23.55]
                    ]]]
                  }
                }
              ]
            }
            """));

        Assert.Equal(1, repasse);
        Assert.Equal(1, await context.SetoresCensitarios.CountAsync());

        var setor = await context.SetoresCensitarios.SingleAsync();
        Assert.Equal("355030800000002", setor.CodigoSetor);
        Assert.Equal(4674, setor.Geometria.SRID);
        Assert.Equal("MultiPolygon", setor.Geometria.GeometryType);
    }

    [Fact]
    public async Task IngerirSetoresCensitariosAsync_ComGeoJsonSemFeatures_NaoApagaCargaExistente()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();

        var pipeline = new IbgeSpatialIngestionPipeline(context);

        await pipeline.IngerirSetoresCensitariosAsync(ToStream("""
            {
              "type": "FeatureCollection",
              "features": [
                {
                  "type": "Feature",
                  "properties": {
                    "cd_setor": "355030800000001",
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
            """));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => pipeline.IngerirSetoresCensitariosAsync(ToStream("""
            {
              "type": "FeatureCollection",
              "features": []
            }
            """)));

        Assert.Contains("evitar perda de dados", ex.Message);
        Assert.Equal(1, await context.SetoresCensitarios.CountAsync());

        var setor = await context.SetoresCensitarios.SingleAsync();
        Assert.Equal("355030800000001", setor.CodigoSetor);
    }

    private PublicDataDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PublicDataDbContext>()
            .UseNpgsql(_postgres.GetConnectionString(), options => options.UseNetTopologySuite())
            .Options;

        return new PublicDataDbContext(options);
    }

    private static MemoryStream ToStream(string csv)
        => new(Encoding.UTF8.GetBytes(csv));
}
