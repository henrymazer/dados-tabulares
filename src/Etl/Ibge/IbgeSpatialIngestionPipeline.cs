using Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Etl.Ibge;

public sealed class IbgeSpatialIngestionPipeline(PublicDataDbContext context)
{
    private readonly IbgeSpatialGeoJsonParser _parser = new();

    public async Task<int> IngerirSetoresCensitariosAsync(Stream geoJson, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(geoJson);

        var setores = _parser.ParseSetoresCensitarios(geoJson);
        if (setores is not { Count: > 0 })
        {
            Log.Warning("Carga espacial do IBGE abortada: o GeoJSON analisado nao produziu setores censitarios.");
            throw new InvalidOperationException("GeoJSON IBGE sem setores censitarios; operacao abortada para evitar perda de dados.");
        }

        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        await context.SetoresCensitarios.ExecuteDeleteAsync(ct);
        context.ChangeTracker.Clear();
        await context.SetoresCensitarios.AddRangeAsync(setores, ct);
        await context.SaveChangesAsync(ct);

        await transaction.CommitAsync(ct);

        return setores.Count;
    }
}
