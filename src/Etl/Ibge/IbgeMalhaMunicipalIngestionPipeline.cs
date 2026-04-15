using Data;
using Microsoft.EntityFrameworkCore;

namespace Etl.Ibge;

public sealed class IbgeMalhaMunicipalIngestionPipeline(PublicDataDbContext context)
{
    private readonly IbgeGeoPackageReader _reader = new();

    public async Task<int> IngerirAsync(string filePath, CancellationToken ct = default)
    {
        var municipios = _reader.ReadMunicipios(filePath);

        await using var transaction = await context.Database.BeginTransactionAsync(ct);
        await context.MunicipiosMalha.ExecuteDeleteAsync(ct);
        context.ChangeTracker.Clear();
        await context.MunicipiosMalha.AddRangeAsync(municipios, ct);
        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return municipios.Count;
    }
}
