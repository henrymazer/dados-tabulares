using System.Globalization;
using System.Text;

namespace Etl.Pnad;

public sealed class PnadCsvParser
{
    public IReadOnlyList<PnadCsvRow> Parse(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var headerLine = ReadMeaningfulLine(reader);
        if (headerLine is null)
            return [];

        var delimiter = DetectDelimiter(headerLine);
        var headers = SplitCsvLine(headerLine, delimiter)
            .Select(NormalizeHeader)
            .ToArray();

        var ufIndex = FindHeader(headers, "uf", "sigla_uf", "ufsigla");
        var anoIndex = FindHeader(headers, "ano");
        var trimestreIndex = FindHeader(headers, "trimestre", "numero_trimestre", "trimestre_numero");
        var valorIndex = FindHeader(headers, "valor", "taxa_desemprego", "taxa_informalidade", "renda_media");

        var rows = new List<PnadCsvRow>();
        string? line;
        while ((line = ReadMeaningfulLine(reader)) is not null)
        {
            var fields = SplitCsvLine(line, delimiter);

            rows.Add(new PnadCsvRow(
                NormalizeUf(GetField(fields, ufIndex)),
                int.Parse(GetField(fields, anoIndex), CultureInfo.InvariantCulture),
                int.Parse(GetField(fields, trimestreIndex), CultureInfo.InvariantCulture),
                ParseDecimal(GetField(fields, valorIndex))));
        }

        return rows;
    }

    private static string? ReadMeaningfulLine(TextReader reader)
    {
        while (true)
        {
            var line = reader.ReadLine();
            if (line is null)
                return null;

            if (!string.IsNullOrWhiteSpace(line))
                return line;
        }
    }

    private static char DetectDelimiter(string headerLine)
        => headerLine.Count(c => c == ';') >= headerLine.Count(c => c == ',') ? ';' : ',';

    private static string[] SplitCsvLine(string line, char delimiter)
    {
        var fields = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var character = line[i];
            if (character == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                    continue;
                }

                inQuotes = !inQuotes;
                continue;
            }

            if (character == delimiter && !inQuotes)
            {
                fields.Add(current.ToString().Trim());
                current.Clear();
                continue;
            }

            current.Append(character);
        }

        fields.Add(current.ToString().Trim());
        return fields.ToArray();
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

    private static int FindHeader(string[] headers, params string[] expectedNames)
    {
        for (var index = 0; index < headers.Length; index++)
        {
            if (expectedNames.Any(expected => headers[index] == NormalizeHeader(expected)))
                return index;
        }

        throw new InvalidOperationException($"CSV PNAD nao contem uma coluna esperada: {string.Join(", ", expectedNames)}.");
    }

    private static string GetField(string[] fields, int index)
    {
        if (index >= fields.Length)
            throw new InvalidOperationException("Linha CSV PNAD com menos colunas que o cabecalho.");

        return fields[index];
    }

    private static string NormalizeUf(string uf)
        => string.IsNullOrWhiteSpace(uf)
            ? throw new InvalidOperationException("UF PNAD nao pode ser vazia.")
            : uf.Trim().ToUpperInvariant();

    private static decimal ParseDecimal(string value)
    {
        var style = NumberStyles.Number;

        if (value.Contains(',') && !value.Contains('.'))
        {
            return decimal.Parse(value, style, CultureInfo.GetCultureInfo("pt-BR"));
        }

        if (decimal.TryParse(value, style, CultureInfo.InvariantCulture, out var parsedInvariant))
        {
            return parsedInvariant;
        }

        return decimal.Parse(value, style, CultureInfo.GetCultureInfo("pt-BR"));
    }
}
