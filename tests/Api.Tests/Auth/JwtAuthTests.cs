using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Api.Tests.Auth;

public class JwtAuthTests : IClassFixture<JwtAuthWebApplicationFactory>
{
    private readonly JwtAuthWebApplicationFactory _factory;

    public JwtAuthTests(JwtAuthWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetMeWithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMeWithExpiredToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(
            expires: DateTimeOffset.UtcNow.AddMinutes(-5)));

        var response = await client.GetAsync("/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMeWithInvalidSignature_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(
            signingKey: JwtAuthWebApplicationFactory.InvalidSigningKey));

        var response = await client.GetAsync("/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMeWithValidToken_ReturnsTenantAndModules()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(
            tenantId: "tenant-123",
            modules: ["ibge", "pnad"]));

        var response = await client.GetAsync("/auth/me");

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<AuthContextResponse>();

        Assert.NotNull(payload);
        Assert.Equal("tenant-123", payload.TenantId);
        Assert.Equal(["ibge", "pnad"], payload.Modules);
    }

    [Fact]
    public async Task GetModuleWithValidTokenWithoutModule_ReturnsForbidden()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(
            modules: ["ibge"]));

        var response = await client.GetAsync("/auth/modules/pnad");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetModuleWithValidTokenAndRequiredModule_ReturnsOk()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(
            tenantId: "tenant-456",
            modules: ["ibge", "pnad"]));

        var response = await client.GetAsync("/auth/modules/pnad");

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<AuthModuleAccessResponse>();

        Assert.NotNull(payload);
        Assert.Equal("pnad", payload.Module);
        Assert.Equal("tenant-456", payload.TenantId);
        Assert.Equal(["ibge", "pnad"], payload.Modules);
    }

    private static string CreateToken(
        string? tenantId = "tenant-001",
        IReadOnlyCollection<string>? modules = null,
        DateTimeOffset? expires = null,
        string? signingKey = null)
    {
        var expiry = expires ?? DateTimeOffset.UtcNow.AddMinutes(15);
        var notBefore = expires is null
            ? DateTime.UtcNow.AddMinutes(-1)
            : expiry.AddMinutes(-10).UtcDateTime;
        var claims = new List<Claim>();

        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            claims.Add(new Claim("tenant_id", tenantId));
        }

        foreach (var module in modules ?? ["ibge", "tse"])
        {
            claims.Add(new Claim("modules", module));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey ?? JwtAuthWebApplicationFactory.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: JwtAuthWebApplicationFactory.Issuer,
            audience: JwtAuthWebApplicationFactory.Audience,
            claims: claims,
            notBefore: notBefore,
            expires: expiry.UtcDateTime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public sealed class JwtAuthWebApplicationFactory : WebApplicationFactory<Program>
{
    public const string Issuer = "dados-tabulares-local";
    public const string Audience = "dados-tabulares-api";
    public const string SigningKey = "development-signing-key-change-me";
    public const string InvalidSigningKey = "invalid-signing-key-01234567890123456789";

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth:Issuer"] = Issuer,
                ["Auth:Audience"] = Audience,
                ["Auth:SigningKey"] = SigningKey,
            });
        });
    }
}

public sealed record AuthContextResponse(string? TenantId, string[] Modules);

public sealed record AuthModuleAccessResponse(string Module, string? TenantId, string[] Modules);
