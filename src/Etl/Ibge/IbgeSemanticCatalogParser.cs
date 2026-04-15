using Microsoft.VisualBasic.FileIO;

namespace Etl.Ibge;

public sealed class IbgeSemanticCatalogParser
{
    public IbgeSemanticCatalog ParseDictionary(Stream input, string sourceName)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceName);

        using var parser = new TextFieldParser(input)
        {
            TextFieldType = FieldType.Delimited,
            HasFieldsEnclosedInQuotes = true,
            TrimWhiteSpace = true
        };

        parser.SetDelimiters(",", ";");

        var headers = parser.ReadFields() ?? throw new InvalidOperationException("Dicionário IBGE sem cabeçalho.");
        var normalizedHeaders = headers.Select(NormalizeHeader).ToArray();
        var pacote = ResolvePackage(sourceName);
        var entries = new List<IbgeSemanticCatalogEntry>();

        while (!parser.EndOfData)
        {
            var fields = parser.ReadFields();
            if (fields is null || fields.All(string.IsNullOrWhiteSpace))
            {
                continue;
            }

            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var index = 0; index < normalizedHeaders.Length && index < fields.Length; index++)
            {
                values[normalizedHeaders[index]] = fields[index].Trim();
            }

            var variavel = values.GetOptional("variavel");
            if (string.IsNullOrWhiteSpace(variavel))
            {
                continue;
            }

            entries.Add(new IbgeSemanticCatalogEntry(
                Path.GetFileName(sourceName),
                pacote,
                values.GetOptional("tipo"),
                values.GetOptional("tema"),
                variavel,
                values.GetOptional("categorias"),
                values.GetRequired("descricao")));
        }

        return new IbgeSemanticCatalog(entries);
    }

    public IbgeSemanticCatalog ParseDirectory(string directoryPath)
    {
        var dictionaryFiles = Directory
            .EnumerateFiles(directoryPath, "*.csv", System.IO.SearchOption.TopDirectoryOnly)
            .Where(path => Path.GetFileName(path).Contains("dicionario", StringComparison.OrdinalIgnoreCase))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (dictionaryFiles.Count == 0)
        {
            throw new InvalidOperationException("Nenhum dicionário IBGE encontrado no diretório informado.");
        }

        var entries = new List<IbgeSemanticCatalogEntry>();
        foreach (var filePath in dictionaryFiles)
        {
            using var stream = File.OpenRead(filePath);
            entries.AddRange(ParseDictionary(stream, Path.GetFileName(filePath)).Entries);
        }

        return new IbgeSemanticCatalog(entries);
    }

    private static string ResolvePackage(string sourceName)
    {
        var normalized = Path.GetFileNameWithoutExtension(sourceName)
            .ToLowerInvariant();

        if (normalized.Contains("geral", StringComparison.Ordinal))
            return "geral";
        if (normalized.Contains("basico", StringComparison.Ordinal))
            return "basico";
        if (normalized.Contains("indigenas", StringComparison.Ordinal))
            return "indigenas";
        if (normalized.Contains("quilombolas", StringComparison.Ordinal))
            return "quilombolas";
        if (normalized.Contains("renda_responsavel", StringComparison.Ordinal))
            return "renda-responsavel";
        if (normalized.Contains("entorno", StringComparison.Ordinal))
            return "entorno-moradores";
        if (normalized.Contains("malha_agregados", StringComparison.Ordinal))
            return "malha-agregados";

        return normalized;
    }

    private static string NormalizeHeader(string header)
    {
        var normalized = header.Trim().ToLowerInvariant();
        normalized = normalized.Replace("á", "a")
            .Replace("à", "a")
            .Replace("ã", "a")
            .Replace("â", "a")
            .Replace("é", "e")
            .Replace("ê", "e")
            .Replace("í", "i")
            .Replace("ó", "o")
            .Replace("ô", "o")
            .Replace("õ", "o")
            .Replace("ú", "u")
            .Replace("ç", "c");

        return new string(normalized.Where(c => c is not '_' and not '-' and not ' ').ToArray());
    }
}

internal static class IbgeSemanticCatalogParserDictionaryExtensions
{
    public static string GetRequired(this IReadOnlyDictionary<string, string> values, string key)
        => values.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new InvalidOperationException($"Campo obrigatório do dicionário IBGE ausente: {key}.");

    public static string? GetOptional(this IReadOnlyDictionary<string, string> values, string key)
        => values.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : null;
}
