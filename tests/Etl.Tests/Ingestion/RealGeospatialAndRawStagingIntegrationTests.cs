using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Data;
using Etl;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Testcontainers.PostgreSql;

namespace Etl.Tests.Ingestion;

public sealed class RealGeospatialAndRawStagingIntegrationTests : IAsyncLifetime
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
    public async Task RunAsync_ComMalhaMunicipalGpkg_PersisteMunicipioERegistraSnapshot()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();

        var gpkgPath = CreateMunicipiosGeoPackage();

        try
        {
            var runner = new EtlRunner(context);
            var imported = await runner.RunAsync(new EtlCommand(EtlSourceKind.Ibge, "malha-municipal", gpkgPath, "malha-municipal"));

            Assert.Equal(1, imported);

            var municipio = await context.MunicipiosMalha.SingleAsync();
            Assert.Equal("4106902", municipio.CodigoMunicipio);
            Assert.Equal("Curitiba", municipio.NomeMunicipio);
            Assert.Equal("PR", municipio.UfSigla);
            Assert.Equal(4674, municipio.Geometria.SRID);

            var snapshot = await context.CargasBrutasSnapshots.SingleAsync(x => x.Dataset == "malha-municipal");
            Assert.True(snapshot.IsCurrent);
            Assert.Equal(1, snapshot.RegistrosImportados);
        }
        finally
        {
            File.Delete(gpkgPath);
        }
    }

    [Fact]
    public async Task RunAsync_ComSetoresCensitariosGpkg_PersisteAtributosOficiaisEChaveTextual()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();

        var gpkgPath = CreateSetoresGeoPackage();

        try
        {
            var runner = new EtlRunner(context);
            var imported = await runner.RunAsync(new EtlCommand(EtlSourceKind.Ibge, "setores-censitarios", gpkgPath, "setores-censitarios"));

            Assert.Equal(1, imported);

            var setor = await context.SetoresCensitarios.SingleAsync();
            Assert.Equal("410690205000001", setor.CodigoSetor);
            Assert.Equal("4106902", setor.MunicipioCodigoIbge);
            Assert.Equal("Curitiba", setor.MunicipioNome);
            Assert.Equal("Batel", setor.NomeBairro);
            Assert.Equal("Urbana", setor.Situacao);
            Assert.Equal(4674, setor.Geometria.SRID);
        }
        finally
        {
            File.Delete(gpkgPath);
        }
    }

    [Fact]
    public async Task RunAsync_ComSetoresCensitariosDuplicadosNoGeoPackage_ConsolidaGeometriasPorCodigoSetor()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();

        var gpkgPath = CreateSetoresGeoPackage(withDuplicateSetor: true);

        try
        {
            var runner = new EtlRunner(context);
            var imported = await runner.RunAsync(new EtlCommand(EtlSourceKind.Ibge, "setores-censitarios", gpkgPath, "setores-censitarios"));

            Assert.Equal(1, imported);
            Assert.Equal(1, await context.SetoresCensitarios.CountAsync());

            var setor = await context.SetoresCensitarios.SingleAsync();
            Assert.Equal("410690205000001", setor.CodigoSetor);
            Assert.Equal(2, setor.Geometria.NumGeometries);
        }
        finally
        {
            File.Delete(gpkgPath);
        }
    }

    [Fact]
    public async Task IngerirAsync_ComLotesPequenos_PersisteTodosOsSetoresEConsolidaDuplicados()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();

        var gpkgPath = CreateSetoresGeoPackage(uniqueSetorCount: 5, withDuplicateSetor: true);

        try
        {
            var pipeline = new IbgeGeoPackageSetorIngestionPipeline(context, batchSize: 2);
            var imported = await pipeline.IngerirAsync(gpkgPath);

            Assert.Equal(5, imported);
            Assert.Equal(5, await context.SetoresCensitarios.CountAsync());

            var primeiroSetor = await context.SetoresCensitarios.SingleAsync(x => x.CodigoSetor == "410690205000001");
            Assert.Equal(2, primeiroSetor.Geometria.NumGeometries);
        }
        finally
        {
            File.Delete(gpkgPath);
        }
    }

    [Fact]
    public async Task RunAsync_ComCatalogoSemanticoDeDicionariosReais_PersisteVariaveisECategorias()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();

        var repoRoot = GetRepositoryRoot();
        var runner = new EtlRunner(context);

        var imported = await runner.RunAsync(new EtlCommand(EtlSourceKind.Ibge, "catalogo-semantico", repoRoot, "catalogo-semantico"));

        Assert.True(imported > 0);

        var geral = await context.IbgeCatalogoVariaveis.SingleAsync(x => x.Pacote == "geral" && x.Variavel == "V00001");
        Assert.Equal("Domicílio", geral.Tipo);
        Assert.Equal("Características do Domicílio - Parte 1", geral.Tema);

        var categoria = await context.IbgeCatalogoCategorias.SingleAsync(x => x.Pacote == "malha-agregados" && x.Variavel == "SITUACAO" && x.Categoria == "Urbana");
        Assert.Equal("Urbana", categoria.Descricao);
    }

    [Fact]
    public async Task RunAsync_ComZipDeAgregadosIbge_PreservaPayloadOriginalESnapshot()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();

        var zipPath = CreateIbgeAggregateZip();

        try
        {
            var runner = new EtlRunner(context);
            var command = new EtlCommand(EtlSourceKind.Ibge, "agregados-staging", zipPath, "agregados_por_setores_basico_br_20250417");

            var firstRun = await runner.RunAsync(command);
            var secondRun = await runner.RunAsync(command);

            Assert.Equal(2, firstRun);
            Assert.Equal(0, secondRun);

            var row = await context.IbgeAgregadosStaging.SingleAsync(x => x.CodigoSetor == "410690205000001");
            using var payload = JsonDocument.Parse(row.PayloadJson);
            Assert.Equal("1", payload.RootElement.GetProperty("V0001").GetString());

            var snapshot = await context.CargasBrutasSnapshots.SingleAsync(x => x.Dataset == "agregados_por_setores_basico_br_20250417");
            Assert.Equal(2, snapshot.RegistrosImportados);
        }
        finally
        {
            File.Delete(zipPath);
        }
    }

    [Fact]
    public async Task RunAsync_ComZipDeLocaisBrutosTse_PreservaCamposRelevantesParaAuditoria()
    {
        await using var context = CreateContext();
        await context.Database.MigrateAsync();

        var zipPath = CreateTseLocaisZip();

        try
        {
            var runner = new EtlRunner(context);
            var imported = await runner.RunAsync(new EtlCommand(EtlSourceKind.Tse, "locais-brutos", zipPath, "detalhe_votacao_secao_2024", 2024));

            Assert.Equal(2, imported);

            var row = await context.TseLocaisVotacaoBrutos.SingleAsync(x => x.NumeroSecao == 101);
            Assert.Equal("PR", row.UfSigla);
            Assert.Equal("4106902", row.MunicipioCodigoIbge);
            Assert.Equal(2755, row.NumeroLocalVotacao);
            Assert.Equal("ESCOLA MUNICIPAL CENTRO", row.NomeLocalVotacao);
            Assert.Contains("RUA DAS FLORES", row.EnderecoLocalVotacao);

            var snapshot = await context.CargasBrutasSnapshots.SingleAsync(x => x.Dataset == "detalhe_votacao_secao_2024");
            Assert.Equal(2, snapshot.RegistrosImportados);
        }
        finally
        {
            File.Delete(zipPath);
        }
    }

    private PublicDataDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PublicDataDbContext>()
            .UseNpgsql(_postgres.GetConnectionString(), options => options.UseNetTopologySuite())
            .Options;

        return new PublicDataDbContext(options);
    }

    private static string CreateMunicipiosGeoPackage()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.gpkg");
        using var connection = new SqliteConnection($"Data Source={path}");
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE br_municipios_2025shp (
                fid INTEGER PRIMARY KEY,
                geom BLOB NOT NULL,
                CD_MUN TEXT,
                NM_MUN TEXT,
                CD_RGI TEXT,
                NM_RGI TEXT,
                CD_RGINT TEXT,
                NM_RGINT TEXT,
                CD_UF TEXT,
                NM_UF TEXT,
                SIGLA_UF TEXT,
                CD_REGIAO TEXT,
                NM_REGIAO TEXT,
                SIGLA_RG TEXT,
                CD_CONCURB TEXT,
                NM_CONCURB TEXT,
                AREA_KM2 REAL
            );
            """;
        command.ExecuteNonQuery();

        using var insert = connection.CreateCommand();
        insert.CommandText = """
            INSERT INTO br_municipios_2025shp
            (geom, CD_MUN, NM_MUN, CD_RGI, NM_RGI, CD_RGINT, NM_RGINT, CD_UF, NM_UF, SIGLA_UF, CD_REGIAO, NM_REGIAO, SIGLA_RG, CD_CONCURB, NM_CONCURB, AREA_KM2)
            VALUES ($geom, '4106902', 'Curitiba', '410001', 'Curitiba', '4101', 'Curitiba', '41', 'Paraná', 'PR', '4', 'Sul', 'S', '4106902', 'Curitiba', 434.89);
            """;
        insert.Parameters.AddWithValue("$geom", CreateGeoPackageBlob(CreatePolygon()));
        insert.ExecuteNonQuery();

        return path;
    }

    private static string CreateSetoresGeoPackage(int uniqueSetorCount = 1, bool withDuplicateSetor = false)
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.gpkg");
        using var connection = new SqliteConnection($"Data Source={path}");
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE BR_setores_CD2022 (
                id INTEGER PRIMARY KEY,
                geom BLOB NOT NULL,
                CD_SETOR TEXT,
                SITUACAO TEXT,
                CD_SIT TEXT,
                CD_TIPO TEXT,
                AREA_KM2 REAL,
                CD_REGIAO TEXT,
                NM_REGIAO TEXT,
                CD_UF TEXT,
                NM_UF TEXT,
                CD_MUN TEXT,
                NM_MUN TEXT,
                CD_DIST TEXT,
                NM_DIST TEXT,
                CD_SUBDIST TEXT,
                NM_SUBDIST TEXT,
                CD_BAIRRO TEXT,
                NM_BAIRRO TEXT,
                CD_NU TEXT,
                NM_NU TEXT,
                CD_FCU TEXT,
                NM_FCU TEXT,
                CD_AGLOM TEXT,
                NM_AGLOM TEXT,
                CD_RGINT TEXT,
                NM_RGINT TEXT,
                CD_RGI TEXT,
                NM_RGI TEXT,
                CD_CONCURB TEXT,
                NM_CONCURB TEXT
            );
            """;
        command.ExecuteNonQuery();

        for (var index = 0; index < uniqueSetorCount; index++)
        {
            InsertSetorRow(
                connection,
                codigoSetor: $"4106902050000{index + 1:00}",
                bairro: $"Bairro {index + 1}",
                geometry: CreatePolygon(-49.28 + (index * 0.02), -25.45));
        }

        if (withDuplicateSetor)
        {
            InsertSetorRow(
                connection,
                codigoSetor: "410690205000001",
                bairro: "Bairro 1",
                geometry: CreatePolygon(-49.26, -25.45));
        }

        return path;
    }

    private static void InsertSetorRow(SqliteConnection connection, string codigoSetor, string bairro, Geometry geometry)
    {
        using var insert = connection.CreateCommand();
        insert.CommandText = """
            INSERT INTO BR_setores_CD2022
            (geom, CD_SETOR, SITUACAO, CD_SIT, CD_TIPO, AREA_KM2, CD_REGIAO, NM_REGIAO, CD_UF, NM_UF, CD_MUN, NM_MUN, CD_DIST, NM_DIST, CD_SUBDIST, NM_SUBDIST, CD_BAIRRO, NM_BAIRRO, CD_NU, NM_NU, CD_FCU, NM_FCU, CD_AGLOM, NM_AGLOM, CD_RGINT, NM_RGINT, CD_RGI, NM_RGI, CD_CONCURB, NM_CONCURB)
            VALUES ($geom, $codigoSetor, 'Urbana', '1', '0', 0.12, '4', 'Sul', '41', 'Paraná', '4106902', 'Curitiba', '410690205', 'Curitiba', '41069020500', 'Centro', '4106902001', $bairro, '.', '.', '.', '.', '.', '.', '4101', 'Curitiba', '410001', 'Curitiba', '4106902', 'Curitiba');
            """;
        insert.Parameters.AddWithValue("$geom", CreateGeoPackageBlob(geometry));
        insert.Parameters.AddWithValue("$codigoSetor", codigoSetor);
        insert.Parameters.AddWithValue("$bairro", bairro);
        insert.ExecuteNonQuery();
    }

    private static string CreateIbgeAggregateZip()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.zip");
        using var archive = ZipFile.Open(path, ZipArchiveMode.Create);
        var entry = archive.CreateEntry("Agregados_por_setores_basico_BR_20250417.csv");
        using var writer = new StreamWriter(entry.Open(), Encoding.UTF8);
        writer.WriteLine("\"CD_SETOR\";\"V0001\";\"V0002\"");
        writer.WriteLine("\"410690205000001\";\"1\";\"2\"");
        writer.WriteLine("\"410690205000002\";\"3\";\"4\"");
        writer.Flush();
        return path;
    }

    private static string CreateTseLocaisZip()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.zip");
        using var archive = ZipFile.Open(path, ZipArchiveMode.Create);
        var entry = archive.CreateEntry("detalhe_votacao_secao_2024_PR.csv");
        using var writer = new StreamWriter(entry.Open(), Encoding.UTF8);
        writer.WriteLine("\"DT_GERACAO\";\"HH_GERACAO\";\"ANO_ELEICAO\";\"CD_TIPO_ELEICAO\";\"NM_TIPO_ELEICAO\";\"NR_TURNO\";\"CD_ELEICAO\";\"DS_ELEICAO\";\"DT_ELEICAO\";\"TP_ABRANGENCIA\";\"SG_UF\";\"SG_UE\";\"NM_UE\";\"CD_MUNICIPIO\";\"NM_MUNICIPIO\";\"NR_ZONA\";\"NR_SECAO\";\"CD_CARGO\";\"DS_CARGO\";\"QT_APTOS\";\"QT_COMPARECIMENTO\";\"QT_ABSTENCOES\";\"QT_VOTOS_NOMINAIS\";\"QT_VOTOS_BRANCOS\";\"QT_VOTOS_NULOS\";\"QT_VOTOS_LEGENDA\";\"QT_VOTOS_ANULADOS_APU_SEP\";\"NR_LOCAL_VOTACAO\";\"NM_LOCAL_VOTACAO\";\"DS_LOCAL_VOTACAO_ENDERECO\"");
        writer.WriteLine("\"06/10/2024\";\"18:00:00\";2024;2;\"Eleição Ordinária\";1;619;\"Eleições Municipais 2024\";\"06/10/2024\";\"M\";\"PR\";\"4106902\";\"CURITIBA\";4106902;\"CURITIBA\";1;101;11;\"Prefeito\";300;250;50;230;10;10;0;0;2755;\"ESCOLA MUNICIPAL CENTRO\";\"RUA DAS FLORES, 10\"");
        writer.WriteLine("\"06/10/2024\";\"18:00:00\";2024;2;\"Eleição Ordinária\";1;619;\"Eleições Municipais 2024\";\"06/10/2024\";\"M\";\"PR\";\"4106902\";\"CURITIBA\";4106902;\"CURITIBA\";1;102;13;\"Vereador\";300;250;50;220;15;15;0;0;2755;\"ESCOLA MUNICIPAL CENTRO\";\"RUA DAS FLORES, 10\"");
        writer.Flush();
        return path;
    }

    private static byte[] CreateGeoPackageBlob(Geometry geometry)
    {
        var wkb = new WKBWriter().Write(geometry);
        var blob = new byte[8 + wkb.Length];
        blob[0] = 0x47;
        blob[1] = 0x50;
        blob[2] = 0;
        blob[3] = 1;
        BitConverter.GetBytes(4674).CopyTo(blob, 4);
        wkb.CopyTo(blob, 8);
        return blob;
    }

    private static Polygon CreatePolygon(double minX = -49.28, double minY = -25.45)
    {
        var factory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4674);
        return factory.CreatePolygon([
            new Coordinate(minX, minY),
            new Coordinate(minX + 0.01, minY),
            new Coordinate(minX + 0.01, minY + 0.01),
            new Coordinate(minX, minY + 0.01),
            new Coordinate(minX, minY)
        ]);
    }

    private static string GetRepositoryRoot()
        => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../.."));
}
