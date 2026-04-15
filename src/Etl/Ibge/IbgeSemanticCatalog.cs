using System.Globalization;
using Microsoft.VisualBasic.FileIO;

namespace Etl.Ibge;

public sealed class IbgeSemanticCatalog
{
    private static readonly string[] CsvHeaders =
    [
        "fonte_dicionario",
        "pacote",
        "tipo",
        "tema",
        "variavel",
        "categoria",
        "descricao"
    ];

    public IbgeSemanticCatalog(IEnumerable<IbgeSemanticCatalogEntry> entries)
    {
        Entries = entries.ToArray();
    }

    public IReadOnlyList<IbgeSemanticCatalogEntry> Entries { get; }

    public void WriteCsv(Stream output)
    {
        ArgumentNullException.ThrowIfNull(output);

        using var writer = new StreamWriter(output, leaveOpen: true);
        writer.WriteLine(string.Join(",", CsvHeaders));

        foreach (var entry in Entries)
        {
            writer.WriteLine(string.Join(",",
                Escape(entry.FonteDicionario),
                Escape(entry.Pacote),
                Escape(entry.Tipo),
                Escape(entry.Tema),
                Escape(entry.Variavel),
                Escape(entry.Categoria),
                Escape(entry.Descricao)));
        }

        writer.Flush();
    }

    public static IbgeSemanticCatalog ReadCsv(Stream input)
    {
        ArgumentNullException.ThrowIfNull(input);

        using var parser = new TextFieldParser(input)
        {
            TextFieldType = FieldType.Delimited,
            HasFieldsEnclosedInQuotes = true,
            TrimWhiteSpace = true
        };

        parser.SetDelimiters(",");

        var headers = parser.ReadFields() ?? throw new InvalidOperationException("Catálogo semântico IBGE sem cabeçalho.");
        var normalizedHeaders = headers.Select(NormalizeHeader).ToArray();
        var entries = new List<IbgeSemanticCatalogEntry>();

        while (!parser.EndOfData)
        {
            var fields = parser.ReadFields();
            if (fields is null || fields.All(string.IsNullOrWhiteSpace))
            {
                continue;
            }

            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < normalizedHeaders.Length && i < fields.Length; i++)
            {
                values[normalizedHeaders[i]] = fields[i].Trim();
            }

            entries.Add(new IbgeSemanticCatalogEntry(
                GetRequired(values, "fontedicionario"),
                GetRequired(values, "pacote"),
                GetOptional(values, "tipo"),
                GetOptional(values, "tema"),
                GetRequired(values, "variavel"),
                GetOptional(values, "categoria"),
                GetRequired(values, "descricao")));
        }

        return new IbgeSemanticCatalog(entries);
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

    private static string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var needsQuotes = value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r');
        var escaped = value.Replace("\"", "\"\"");
        return needsQuotes ? $"\"{escaped}\"" : escaped;
    }

    private static string GetRequired(IReadOnlyDictionary<string, string> values, string header)
    {
        if (values.TryGetValue(header, out var value) && !string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        throw new InvalidOperationException($"Campo obrigatório ausente no catálogo IBGE: {header}.");
    }

    private static string? GetOptional(IReadOnlyDictionary<string, string> values, string header)
        => values.TryGetValue(header, out var value) && !string.IsNullOrWhiteSpace(value) ? value : null;
}
