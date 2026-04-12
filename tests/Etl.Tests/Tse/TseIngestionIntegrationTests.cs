using Data;
using DadosTabulares.Domain.Tse;
using DadosTabulares.Etl.Tse;
using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace Etl.Tests.Tse;

public sealed class TseIngestionIntegrationTests : IAsyncLifetime
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
    public async Task IngerirResultadosAsync_IsIdempotent()
    {
        await using var context = CreateContext();
        await ResetDatabaseAsync(context);

        var service = CreateService(context);
        var csv = """
                  NR_CPF_CANDIDATO;NM_CANDIDATO;NM_URNA_CANDIDATO;NR_CANDIDATO;DS_CARGO;NR_PARTIDO;SG_PARTIDO;NM_PARTIDO;SG_UF;CD_MUNICIPIO;NM_MUNICIPIO;NR_ZONA;NR_SECAO;QT_VOTOS
                  12345678900;Maria Silva;Maria;13;Prefeita;13;PT;Partido dos Trabalhadores;SP;3550308;Sao Paulo;1;101;1234
                  """;

        var firstLoad = await service.IngerirResultadosAsync(new StringReader(csv), new AnoEleicao(2024));
        var secondLoad = await service.IngerirResultadosAsync(new StringReader(csv), new AnoEleicao(2024));

        Assert.Equal(1, firstLoad);
        Assert.Equal(0, secondLoad);
        Assert.Equal(1, await context.ResultadosEleitorais.CountAsync());
    }

    [Fact]
    public async Task IngerirCadaDatasetAsync_PopulatesOnlyItsOwnTable()
    {
        await using var context = CreateContext();
        await ResetDatabaseAsync(context);

        var service = CreateService(context);

        await service.IngerirResultadosAsync(new StringReader("""
            NR_CPF_CANDIDATO;NM_CANDIDATO;NM_URNA_CANDIDATO;NR_CANDIDATO;DS_CARGO;NR_PARTIDO;SG_PARTIDO;NM_PARTIDO;SG_UF;CD_MUNICIPIO;NM_MUNICIPIO;NR_ZONA;NR_SECAO;QT_VOTOS
            12345678900;Maria Silva;Maria;13;Prefeita;13;PT;Partido dos Trabalhadores;SP;3550308;Sao Paulo;1;101;1234
            """), new AnoEleicao(2024));

        await service.IngerirEleitoradoAsync(new StringReader("""
            SG_UF;CD_MUNICIPIO;NM_MUNICIPIO;NR_ZONA;DS_FAIXA_ETARIA;DS_GRAU_ESCOLARIDADE;DS_GENERO;QT_ELEITORES_PERFIL
            SP;3550308;Sao Paulo;1;25 a 34 anos;Ensino superior completo;Feminino;250
            """), new AnoEleicao(2024));

        await service.IngerirBensAsync(new StringReader("""
            NR_CPF_CANDIDATO;NM_CANDIDATO;NM_URNA_CANDIDATO;NR_CANDIDATO;DS_CARGO;NR_PARTIDO;SG_PARTIDO;NM_PARTIDO;SG_UF;DS_TIPO_BEM_CANDIDATO;DS_BEM_CANDIDATO;VR_BEM_CANDIDATO
            12345678900;Maria Silva;Maria;13;Prefeita;13;PT;Partido dos Trabalhadores;SP;Imóvel;Apartamento em São Paulo;500000
            """), new AnoEleicao(2024));

        await service.IngerirPrestacaoContasAsync(new StringReader("""
            NR_CPF_CANDIDATO;NM_CANDIDATO;NM_URNA_CANDIDATO;NR_CANDIDATO;DS_CARGO;NR_PARTIDO;SG_PARTIDO;NM_PARTIDO;SG_UF;DS_TIPO_RECEITA;DS_RECEITA;VR_RECEITA;DS_TIPO_MOVIMENTO
            12345678900;Maria Silva;Maria;13;Prefeita;13;PT;Partido dos Trabalhadores;SP;Doação pessoa física;Doação campanha;1000;Receita
            """), new AnoEleicao(2024));

        await service.IngerirColigacoesAsync(new StringReader("""
            NM_COLIGACAO;NR_PARTIDO;SG_PARTIDO;NM_PARTIDO
            Brasil da Esperança;13;PT;Partido dos Trabalhadores
            Brasil da Esperança;65;PCdoB;Partido Comunista do Brasil
            """), new AnoEleicao(2024));

        Assert.Equal(1, await context.ResultadosEleitorais.CountAsync());
        Assert.Equal(1, await context.PerfisEleitorado.CountAsync());
        Assert.Equal(1, await context.BensDeclarados.CountAsync());
        Assert.Equal(1, await context.PrestacoesContas.CountAsync());
        Assert.Equal(1, await context.Coligacoes.CountAsync());
    }

    private TseIngestionService CreateService(PublicDataDbContext context)
        => new(new TseCsvParser(), new TseIngestionLoader(context));

    private PublicDataDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PublicDataDbContext>()
            .UseNpgsql(_postgresContainer.GetConnectionString(), options => options.UseNetTopologySuite())
            .Options;

        return new PublicDataDbContext(options);
    }

    private static async Task ResetDatabaseAsync(PublicDataDbContext context)
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }
}
