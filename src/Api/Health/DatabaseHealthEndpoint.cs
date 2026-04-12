using Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Health;

public static class DatabaseHealthEndpoint
{
    public static IEndpointRouteBuilder MapDatabaseHealthEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/health", HandleAsync)
            .AllowAnonymous();

        return endpoints;
    }

    private static async Task<IResult> HandleAsync(
        PublicDataDbContext dbContext,
        CancellationToken ct)
    {
        try
        {
            if (await dbContext.Database.CanConnectAsync(ct))
            {
                return TypedResults.Ok(new HealthResponse("healthy"));
            }
        }
        catch (Exception)
        {
            // Any connectivity exception is mapped to the same unhealthy response.
        }

        return TypedResults.Json(new HealthResponse("unhealthy"), statusCode: StatusCodes.Status503ServiceUnavailable);
    }
}

public sealed record HealthResponse(string Status);
