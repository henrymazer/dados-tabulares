using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Data.Infrastructure;

public sealed class PublicDataDbContextFactory : IDesignTimeDbContextFactory<PublicDataDbContext>
{
    public PublicDataDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<PublicDataDbContext>();
        builder.UseNpgsql("Host=localhost;Port=5432;Database=dados_publicos;Username=postgres;Password=postgres");
        return new PublicDataDbContext(builder.Options);
    }
}
