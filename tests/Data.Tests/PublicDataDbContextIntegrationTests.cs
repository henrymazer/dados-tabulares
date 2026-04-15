using Data;
using Data.Repositories.Ibge;
using Data.Repositories.Pnad;
using Data.Repositories.Tse;
using DadosTabulares.Domain.Ibge;
using DadosTabulares.Domain.Pnad;
using DadosTabulares.Domain.Tse;
using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Npgsql;
using Testcontainers.PostgreSql;

namespace Data.Tests;

public sealed class PublicDataDbContextIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgis/postgis:17-3.5")
        .WithDatabase("dados_publicos")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
    }

    [Fact]
    public async Task MigrateAsync_CreatesExpectedSchemas()
    {
        await using var context = CreateContext();

        await context.Database.MigrateAsync();

        await using var connection = new NpgsqlConnection(_postgresContainer.GetConnectionString());
        await connection.OpenAsync();

        var existingSchemas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        await using var command = new NpgsqlCommand(
            """
            select schema_name
            from information_schema.schemata
            where schema_name in ('ibge', 'tse', 'pnad', 'etl')
            """,
            connection);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            existingSchemas.Add(reader.GetString(0));
        }

        Assert.Equal(4, existingSchemas.Count);
        Assert.Contains("ibge", existingSchemas);
        Assert.Contains("tse", existingSchemas);
        Assert.Contains("pnad", existingSchemas);
        Assert.Contains("etl", existingSchemas);
    }

    [Fact]
    public async Task SpatialModel_PersistsSetorCensitarioGeometry()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();

        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4674);
        var polygon = geometryFactory.CreatePolygon(
        [
            new Coordinate(-46.6400, -23.5500),
            new Coordinate(-46.6390, -23.5500),
            new Coordinate(-46.6390, -23.5490),
            new Coordinate(-46.6400, -23.5490),
            new Coordinate(-46.6400, -23.5500)
        ]);

        context.SetoresCensitarios.Add(new SetorCensitarioRecord
        {
            CodigoSetor = "355030800000001",
            Situacao = string.Empty,
            CodigoSituacao = string.Empty,
            CodigoTipo = string.Empty,
            AreaKm2 = 0d,
            CodigoRegiao = string.Empty,
            NomeRegiao = string.Empty,
            CodigoUf = string.Empty,
            NomeUf = string.Empty,
            MunicipioCodigoIbge = "3550308",
            MunicipioNome = "Sao Paulo",
            CodigoDistrito = string.Empty,
            NomeDistrito = string.Empty,
            CodigoSubdistrito = string.Empty,
            NomeSubdistrito = string.Empty,
            CodigoBairro = string.Empty,
            NomeBairro = string.Empty,
            CodigoNucleoUrbano = string.Empty,
            NomeNucleoUrbano = string.Empty,
            CodigoFcu = string.Empty,
            NomeFcu = string.Empty,
            CodigoAglomerado = string.Empty,
            NomeAglomerado = string.Empty,
            CodigoRegiaoIntermediaria = string.Empty,
            NomeRegiaoIntermediaria = string.Empty,
            CodigoRegiaoImediata = string.Empty,
            NomeRegiaoImediata = string.Empty,
            CodigoConcentracaoUrbana = string.Empty,
            NomeConcentracaoUrbana = string.Empty,
            UfSigla = "SP",
            Geometria = geometryFactory.CreateMultiPolygon([polygon])
        });

        await context.SaveChangesAsync();

        var persisted = await context.SetoresCensitarios.SingleAsync(x => x.CodigoSetor == "355030800000001");

        Assert.Equal(4674, persisted.Geometria.SRID);
        Assert.Equal("MultiPolygon", persisted.Geometria.GeometryType);
        Assert.True(persisted.Geometria.Area > 0);
    }

    [Fact]
    public async Task Repositories_RoundTripDataAcrossAllSchemas()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();

        var resultadoRepository = new ResultadoEleitoralRepository(context);
        var dadoPopulacionalRepository = new DadoPopulacionalRepository(context);
        var dadoDesempregoRepository = new DadoDesempregoRepository(context);

        var uf = new UF("SP");
        var anoEleicao = new AnoEleicao(2024);
        var municipio = new Municipio(3550308, "Sao Paulo", uf);
        var partido = new Partido(10, "PRB", "Partido Republicano Brasileiro");
        var candidato = new Candidato("12345678900", "Maria Silva", "Maria", 10, "Prefeito", partido, anoEleicao, uf);
        var zona = new ZonaEleitoral(1, municipio);
        var secao = new SecaoEleitoral(101, zona);

        await resultadoRepository.AdicionarEmLoteAsync(
        [
            new ResultadoEleitoral(candidato, anoEleicao, 1, zona, secao, 1000)
        ]);

        await dadoPopulacionalRepository.AdicionarEmLoteAsync(
        [
            new DadoPopulacional(municipio, "18-24", "Branca", 2500)
        ]);

        var trimestre = new Trimestre(2024, 1);
        await dadoDesempregoRepository.AdicionarEmLoteAsync(
        [
            new DadoDesemprego(uf, trimestre, 7.5m)
        ]);

        var resultadosMunicipio = await resultadoRepository.ObterPorMunicipioAsync(municipio.CodigoIbge, anoEleicao);
        var dadosPopulacionaisMunicipio = await dadoPopulacionalRepository.ObterPorMunicipioAsync(municipio.CodigoIbge);
        var dadosDesempregoUf = await dadoDesempregoRepository.ObterPorUfETrimestreAsync(uf, trimestre);

        var resultado = Assert.Single(resultadosMunicipio);
        Assert.Equal(1000, resultado.QuantidadeVotos);
        Assert.Equal("12345678900", resultado.Candidato.Cpf);
        Assert.Equal("SP", resultado.Candidato.UF.Sigla);

        var dadoPopulacional = Assert.Single(dadosPopulacionaisMunicipio);
        Assert.Equal("18-24", dadoPopulacional.FaixaEtaria);
        Assert.Equal(2500, dadoPopulacional.Quantidade);

        var dadoDesemprego = Assert.Single(dadosDesempregoUf);
        Assert.Equal(7.5m, dadoDesemprego.TaxaDesemprego);
        Assert.Equal(2024, dadoDesemprego.Trimestre.Ano);
        Assert.Equal(1, dadoDesemprego.Trimestre.Numero);
    }

    [Fact]
    public async Task PersistenceModels_SupportBasicUpdateAndDelete()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();

        context.ResultadosEleitorais.Add(new ResultadoEleitoralRecord
        {
            CandidatoCpf = "98765432100",
            AnoEleicao = 2024,
            CandidatoNome = "Joao Souza",
            CandidatoNomeUrna = "Joao",
            CandidatoNumero = 20,
            Cargo = "Vereador",
            PartidoNumero = 20,
            PartidoSigla = "PSC",
            PartidoNome = "Partido Social Cristao",
            UfSigla = "SP",
            Turno = 1,
            NumeroZona = 2,
            MunicipioCodigoIbge = 3550308,
            MunicipioNome = "Sao Paulo",
            NumeroSecao = 202,
            QuantidadeVotos = 300
        });

        await context.SaveChangesAsync();

        var persisted = await context.ResultadosEleitorais.SingleAsync(x =>
            x.CandidatoCpf == "98765432100" &&
            x.AnoEleicao == 2024 &&
            x.Turno == 1 &&
            x.NumeroZona == 2 &&
            x.MunicipioCodigoIbge == 3550308 &&
            x.NumeroSecao == 202);

        persisted.QuantidadeVotos = 450;
        await context.SaveChangesAsync();

        var updated = await context.ResultadosEleitorais.SingleAsync(x =>
            x.CandidatoCpf == "98765432100" &&
            x.AnoEleicao == 2024 &&
            x.Turno == 1 &&
            x.NumeroZona == 2 &&
            x.MunicipioCodigoIbge == 3550308 &&
            x.NumeroSecao == 202);

        Assert.Equal(450, updated.QuantidadeVotos);

        context.ResultadosEleitorais.Remove(updated);
        await context.SaveChangesAsync();

        var exists = await context.ResultadosEleitorais.AnyAsync(x =>
            x.CandidatoCpf == "98765432100" &&
            x.AnoEleicao == 2024 &&
            x.Turno == 1 &&
            x.NumeroZona == 2 &&
            x.MunicipioCodigoIbge == 3550308 &&
            x.NumeroSecao == 202);

        Assert.False(exists);
    }

    private PublicDataDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PublicDataDbContext>()
            .UseNpgsql(_postgresContainer.GetConnectionString(), options => options.UseNetTopologySuite())
            .Options;

        return new PublicDataDbContext(options);
    }
}
