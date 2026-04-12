using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Infrastructure;

public static class PublicDataServiceCollectionExtensions
{
    public static IServiceCollection AddPublicDataReadOnlyDbContext(this IServiceCollection services, IConfiguration configuration)
        => services.AddPublicDataDbContext(configuration, DatabaseConnectionStringNames.ReadOnly);

    public static IServiceCollection AddPublicDataReadWriteDbContext(this IServiceCollection services, IConfiguration configuration)
        => services.AddPublicDataDbContext(configuration, DatabaseConnectionStringNames.ReadWrite);

    private static IServiceCollection AddPublicDataDbContext(this IServiceCollection services, IConfiguration configuration, string connectionStringName)
    {
        var connectionString = configuration.GetConnectionString(connectionStringName)
            ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' não foi configurada.");

        services.AddDbContext<PublicDataDbContext>(options => options.UseNpgsql(connectionString));
        return services;
    }
}
