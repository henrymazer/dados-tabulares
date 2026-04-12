using Data;
using DadosTabulares.Domain.Tse;
using DadosTabulares.Etl.Tse;
using Etl.Ibge;
using Etl.Pnad;

namespace Etl;

public sealed class EtlRunner(PublicDataDbContext context)
{
    public async Task<int> RunAsync(EtlCommand command, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        return command.Kind switch
        {
            EtlSourceKind.Tse => await RunTseAsync(command, ct),
            EtlSourceKind.Ibge => await RunIbgeAsync(command, ct),
            EtlSourceKind.Pnad => await RunPnadAsync(command, ct),
            _ => throw new ArgumentOutOfRangeException(nameof(command), command.Kind, "Fonte ETL não suportada.")
        };
    }

    private async Task<int> RunTseAsync(EtlCommand command, CancellationToken ct)
    {
        using var reader = File.OpenText(command.FilePath);
        var service = new TseIngestionService(new TseCsvParser(), new TseIngestionLoader(context));
        var source = ParseTseSource(command.Source);

        return await service.IngerirAsync(source, reader, new AnoEleicao(command.Year ?? throw new InvalidOperationException("Ano TSE ausente.")), ct);
    }

    private async Task<int> RunIbgeAsync(EtlCommand command, CancellationToken ct)
    {
        await using var stream = File.OpenRead(command.FilePath);
        var dataset = ParseIbgeDataset(command.Source);

        if (dataset == IbgeDataset.SetoresCensitarios)
        {
            var spatialPipeline = new IbgeSpatialIngestionPipeline(context);
            return await spatialPipeline.IngerirSetoresCensitariosAsync(stream, ct);
        }

        var pipeline = new IbgeIngestionPipeline(context);
        return await pipeline.IngerirAsync(dataset, stream, ct);
    }

    private async Task<int> RunPnadAsync(EtlCommand command, CancellationToken ct)
    {
        await using var stream = File.OpenRead(command.FilePath);
        var pipeline = new PnadIngestionPipeline(context);

        return await pipeline.IngerirAsync(ParsePnadDataset(command.Source), stream, ct);
    }

    private static TseIngestionSource ParseTseSource(string value)
        => Enum.TryParse<TseIngestionSource>(NormalizeEnumToken(value), true, out var parsed)
            ? parsed
            : throw new InvalidOperationException($"Dataset TSE não suportado: '{value}'.");

    private static IbgeDataset ParseIbgeDataset(string value)
        => Enum.TryParse<IbgeDataset>(NormalizeEnumToken(value), true, out var parsed)
            ? parsed
            : throw new InvalidOperationException($"Dataset IBGE não suportado: '{value}'.");

    private static PnadDataset ParsePnadDataset(string value)
        => Enum.TryParse<PnadDataset>(NormalizeEnumToken(value), true, out var parsed)
            ? parsed
            : throw new InvalidOperationException($"Dataset PNAD não suportado: '{value}'.");

    private static string NormalizeEnumToken(string value)
        => value.Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal);
}
