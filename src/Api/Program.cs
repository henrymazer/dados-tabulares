using Api.Auth;
using Api.Health;
using Data.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddPublicDataReadOnlyDbContext(builder.Configuration);
builder.Services.AddLocalJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy(AuthPolicies.ModuleAccess, policy =>
        policy.AddRequirements(new ModuleAccessRequirement()));
});

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapDatabaseHealthEndpoint();

app.Run();

public partial class Program;
