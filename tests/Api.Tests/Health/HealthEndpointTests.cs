using System.Net;
using Api.Tests.Auth;
using Data.Infrastructure;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace Api.Tests.Health;

public sealed class HealthEndpointTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:17-alpine")
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
    public async Task GetHealth_WithAccessibleDatabase_ReturnsHealthy()
    {
        await using var factory = new ConfigurableDatabaseWebApplicationFactory(_postgresContainer.GetConnectionString());
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        response.EnsureSuccessStatusCode();
        Assert.Equal("""{"status":"healthy"}""", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task GetHealth_WithInaccessibleDatabase_ReturnsServiceUnavailable()
    {
        await using var factory = new ConfigurableDatabaseWebApplicationFactory("Host=127.0.0.1;Port=65432;Database=dados_publicos;Username=postgres;Password=postgres;Timeout=1;Command Timeout=1");
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal("""{"status":"unhealthy"}""", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task GetHealth_DoesNotRequireAuthentication()
    {
        await using var factory = new ConfigurableDatabaseWebApplicationFactory(_postgresContainer.GetConnectionString());
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }
}

public sealed class ConfigurableDatabaseWebApplicationFactory(string readOnlyConnectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"ConnectionStrings:{DatabaseConnectionStringNames.ReadOnly}"] = readOnlyConnectionString,
                ["Auth:Issuer"] = JwtAuthWebApplicationFactory.Issuer,
                ["Auth:Audience"] = JwtAuthWebApplicationFactory.Audience,
                ["Auth:SigningKey"] = JwtAuthWebApplicationFactory.SigningKey,
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<Data.PublicDataDbContext>>();
            services.RemoveAll<Data.PublicDataDbContext>();
            services.AddDbContext<Data.PublicDataDbContext>(options =>
                options.UseNpgsql(readOnlyConnectionString, npgsqlOptions => npgsqlOptions.UseNetTopologySuite()));
        });
    }
}
