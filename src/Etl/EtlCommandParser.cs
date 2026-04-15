using DadosTabulares.Etl.Tse;
using Etl.Ibge;
using Etl.Pnad;

namespace Etl;

public static class EtlCommandParser
{
    public static EtlCommand Parse(string[] args)
    {
        var options = ParseOptions(args);
        var source = GetRequired(options, "source").ToLowerInvariant();
        var filePath = GetRequired(options, "file");
        var year = ParseOptionalInt(options.GetValueOrDefault("year"), "year");
        var normalizedSource = NormalizeEnumToken(source);

        return source switch
        {
            "tse" => CreateTseCommand(filePath, GetRequired(options, "dataset").ToLowerInvariant(), year),
            "ibge" => CreateIbgeCommand(filePath, GetRequired(options, "dataset").ToLowerInvariant()),
            "pnad" => CreatePnadCommand(filePath, GetRequired(options, "dataset").ToLowerInvariant()),
            _ when Enum.TryParse<TseIngestionSource>(normalizedSource, true, out _) => CreateTseCommand(filePath, source, year),
            _ when Enum.TryParse<IbgeDataset>(normalizedSource, true, out _) => CreateIbgeCommand(filePath, source),
            _ when Enum.TryParse<PnadDataset>(normalizedSource, true, out _) => CreatePnadCommand(filePath, source),
            _ => throw new InvalidOperationException($"Fonte ETL não suportada: '{source}'.")
        };
    }

    private static EtlCommand CreateTseCommand(string filePath, string dataset, int? year)
    {
        if (year is null)
        {
            throw new InvalidOperationException("O argumento --year é obrigatório para ingestão TSE.");
        }

        if (!Enum.TryParse<TseIngestionSource>(NormalizeEnumToken(dataset), true, out _))
        {
            throw new InvalidOperationException($"Dataset TSE não suportado: '{dataset}'.");
        }

        return new EtlCommand(
            EtlSourceKind.Tse,
            dataset,
            filePath,
            RequiresFileScopedDataset(ParseTseDataset(dataset)) ? BuildFileScopedDatasetName(filePath) : dataset,
            year);
    }

    private static EtlCommand CreateIbgeCommand(string filePath, string dataset)
    {
        if (!Enum.TryParse<IbgeDataset>(NormalizeEnumToken(dataset), true, out _))
        {
            throw new InvalidOperationException($"Dataset IBGE não suportado: '{dataset}'.");
        }

        return new EtlCommand(
            EtlSourceKind.Ibge,
            dataset,
            filePath,
            RequiresFileScopedDataset(ParseIbgeDataset(dataset)) ? BuildFileScopedDatasetName(filePath) : dataset);
    }

    private static EtlCommand CreatePnadCommand(string filePath, string dataset)
    {
        if (!Enum.TryParse<PnadDataset>(NormalizeEnumToken(dataset), true, out _))
        {
            throw new InvalidOperationException($"Dataset PNAD não suportado: '{dataset}'.");
        }

        return new EtlCommand(EtlSourceKind.Pnad, dataset, filePath, dataset);
    }

    private static Dictionary<string, string> ParseOptions(string[] args)
    {
        var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        for (var index = 0; index < args.Length; index += 2)
        {
            var key = args[index];
            if (!key.StartsWith("--", StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Argumento inválido: '{key}'.");
            }

            if (index + 1 >= args.Length)
            {
                throw new InvalidOperationException($"Valor ausente para argumento '{key}'.");
            }

            options[key[2..]] = args[index + 1];
        }

        return options;
    }

    private static string GetRequired(IReadOnlyDictionary<string, string> options, string key)
        => options.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new InvalidOperationException($"O argumento --{key} é obrigatório.");

    private static int? ParseOptionalInt(string? value, string key)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (int.TryParse(value, out var parsed))
        {
            return parsed;
        }

        throw new InvalidOperationException($"O argumento --{key} deve ser inteiro.");
    }

    private static string NormalizeEnumToken(string value)
        => value.Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal);

    private static IbgeDataset ParseIbgeDataset(string value)
        => Enum.TryParse<IbgeDataset>(NormalizeEnumToken(value), true, out var parsed)
            ? parsed
            : throw new InvalidOperationException($"Dataset IBGE não suportado: '{value}'.");

    private static TseIngestionSource ParseTseDataset(string value)
        => Enum.TryParse<TseIngestionSource>(NormalizeEnumToken(value), true, out var parsed)
            ? parsed
            : throw new InvalidOperationException($"Dataset TSE não suportado: '{value}'.");

    private static bool RequiresFileScopedDataset(IbgeDataset dataset)
        => dataset is IbgeDataset.AgregadosStaging or IbgeDataset.CatalogoSemantico;

    private static bool RequiresFileScopedDataset(TseIngestionSource dataset)
        => dataset is TseIngestionSource.LocaisBrutos;

    private static string BuildFileScopedDatasetName(string filePath)
        => Path.GetFileNameWithoutExtension(filePath).ToLowerInvariant();
}
