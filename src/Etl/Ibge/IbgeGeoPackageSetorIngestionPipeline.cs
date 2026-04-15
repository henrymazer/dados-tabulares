using Data;
using Microsoft.EntityFrameworkCore;

namespace Etl.Ibge;

public sealed class IbgeGeoPackageSetorIngestionPipeline(PublicDataDbContext context, int batchSize = 500)
{
    private readonly IbgeGeoPackageReader _reader = new();
    private readonly int _batchSize = batchSize > 0 ? batchSize : throw new ArgumentOutOfRangeException(nameof(batchSize));

    public async Task<int> IngerirAsync(string filePath, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        await using var transaction = await context.Database.BeginTransactionAsync(ct);
        await context.SetoresCensitarios.ExecuteDeleteAsync(ct);
        context.ChangeTracker.Clear();

        var imported = 0;
        var previousAutoDetectChanges = context.ChangeTracker.AutoDetectChangesEnabled;
        context.ChangeTracker.AutoDetectChangesEnabled = false;

        try
        {
            var batch = new List<SetorCensitarioRecord>(_batchSize);
            foreach (var setor in _reader.StreamSetores(filePath))
            {
                ct.ThrowIfCancellationRequested();
                batch.Add(setor);

                if (batch.Count < _batchSize)
                {
                    continue;
                }

                imported += await PersistBatchAsync(batch, ct);
            }

            imported += await PersistBatchAsync(batch, ct);
            await transaction.CommitAsync(ct);
            return imported;
        }
        finally
        {
            context.ChangeTracker.AutoDetectChangesEnabled = previousAutoDetectChanges;
        }
    }

    private async Task<int> PersistBatchAsync(List<SetorCensitarioRecord> batch, CancellationToken ct)
    {
        if (batch.Count == 0)
        {
            return 0;
        }

        await context.SetoresCensitarios.AddRangeAsync(batch, ct);
        await context.SaveChangesAsync(ct);
        context.ChangeTracker.Clear();

        var imported = batch.Count;
        batch.Clear();
        return imported;
    }
}
