using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Api.Auth;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddLocalJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(JwtAuthOptions.SectionName).Get<JwtAuthOptions>() ?? new JwtAuthOptions();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey));

        services.AddSingleton(options);
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwt =>
            {
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = !string.IsNullOrWhiteSpace(options.Issuer),
                    ValidIssuer = options.Issuer,
                    ValidateAudience = !string.IsNullOrWhiteSpace(options.Audience),
                    ValidAudience = options.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role,
                };
            });

        services.AddAuthorization();
        services.AddSingleton<IAuthorizationHandler, ModuleAccessHandler>();

        return services;
    }

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/auth");

        group.MapGet("/me", (ClaimsPrincipal user) =>
            Results.Ok(new AuthContextResponse(
                user.GetTenantId(),
                user.GetAuthorizedModules().ToArray())))
            .RequireAuthorization();

        group.MapGet("/modules/{module}", (string module, ClaimsPrincipal user) =>
            Results.Ok(new AuthModuleAccessResponse(
                module,
                user.GetTenantId(),
                user.GetAuthorizedModules().ToArray())))
            .RequireAuthorization(AuthPolicies.ModuleAccess);

        return endpoints;
    }
}

public static class AuthPolicies
{
    public const string ModuleAccess = "auth-module-access";
}

public static class AuthClaimTypes
{
    public const string TenantId = "tenant_id";
    public const string Tenant = "tenant";
    public const string Modules = "modules";
    public const string Module = "module";
}

public sealed class JwtAuthOptions
{
    public const string SectionName = "Auth";

    public string Issuer { get; set; } = "dados-tabulares-local";
    public string Audience { get; set; } = "dados-tabulares-api";
    public string SigningKey { get; set; } = "development-signing-key-change-me";
}

public sealed record AuthContextResponse(string? TenantId, string[] Modules);

public sealed record AuthModuleAccessResponse(string Module, string? TenantId, string[] Modules);

public sealed class ModuleAccessRequirement : IAuthorizationRequirement;

public sealed class ModuleAccessHandler : AuthorizationHandler<ModuleAccessRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ModuleAccessRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            return Task.CompletedTask;
        }

        var requiredModule = httpContext.GetRouteValue("module")?.ToString();
        if (string.IsNullOrWhiteSpace(requiredModule))
        {
            return Task.CompletedTask;
        }

        var authorizedModules = httpContext.User.GetAuthorizedModules();
        if (authorizedModules.Any(module => string.Equals(module, requiredModule, StringComparison.OrdinalIgnoreCase)))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public static class ClaimsPrincipalExtensions
{
    public static string? GetTenantId(this ClaimsPrincipal principal)
        => principal.FindFirst(AuthClaimTypes.TenantId)?.Value
            ?? principal.FindFirst(AuthClaimTypes.Tenant)?.Value;

    public static IReadOnlyCollection<string> GetAuthorizedModules(this ClaimsPrincipal principal)
    {
        return principal.Claims
            .Where(claim => string.Equals(claim.Type, AuthClaimTypes.Modules, StringComparison.OrdinalIgnoreCase)
                || string.Equals(claim.Type, AuthClaimTypes.Module, StringComparison.OrdinalIgnoreCase))
            .SelectMany(claim => claim.Value.Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
