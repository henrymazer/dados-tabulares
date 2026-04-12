using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Data.Infrastructure;

public sealed class PublicDataDbContextFactory : IDesignTimeDbContextFactory<PublicDataDbContext>
{
    public PublicDataDbContext CreateDbContext(string[] args)
    {
        const string connectionStringEnvironmentVariable = "ConnectionStrings__ReadWrite";

        var connectionString = Environment.GetEnvironmentVariable(connectionStringEnvironmentVariable);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Set '{connectionStringEnvironmentVariable}' before running design-time EF commands.");
        }

        var builder = new DbContextOptionsBuilder<PublicDataDbContext>();
        builder.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.UseNetTopologySuite());
        return new PublicDataDbContext(builder.Options);
    }
}
