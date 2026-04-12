using System.Globalization;
using System.Text;
using System.Text.Json;
using DadosTabulares.Domain.Tse;

namespace DadosTabulares.Etl.Tse;

public sealed class TseCsvParser
{
    public async Task<IReadOnlyList<ResultadoEleitoral>> ParseResultadosAsync(
        TextReader reader,
        AnoEleicao anoEleicao,
        CultureInfo? culture = null,
        CancellationToken ct = default)
    {
        var rows = await ReadRowsAsync(reader, ct);
        return rows.Select(row =>
        {
            var uf = new UF(GetRequiredString(row, "SG_UF", "UF"));
            var municipio = new Municipio(
                GetRequiredInt(row, "CD_MUNICIPIO", "CODIGO_IBGE", "COD_MUNICIPIO"),
                GetRequiredString(row, "NM_MUNICIPIO", "MUNICIPIO"),
                uf);
            var zona = new ZonaEleitoral(GetRequiredInt(row, "NR_ZONA", "ZONA"), municipio);
            var secao = new SecaoEleitoral(GetRequiredInt(row, "NR_SECAO", "SECAO"), zona);
            var partido = new Partido(
                GetRequiredInt(row, "NR_PARTIDO", "NUMERO_PARTIDO"),
                GetRequiredString(row, "SG_PARTIDO"),
                GetRequiredString(row, "NM_PARTIDO"));
            var candidato = new Candidato(
                GetRequiredString(row, "NR_CPF_CANDIDATO", "CPF_CANDIDATO", "CPF"),
                GetRequiredString(row, "NM_CANDIDATO", "NOME_CANDIDATO"),
                GetRequiredString(row, "NM_URNA_CANDIDATO", "NOME_URNA_CANDIDATO", "NM_URNA"),
                GetRequiredInt(row, "NR_CANDIDATO", "NUMERO_CANDIDATO"),
                GetRequiredString(row, "DS_CARGO", "CARGO"),
                partido,
                anoEleicao,
                uf);

            return new ResultadoEleitoral(
                candidato,
                anoEleicao,
                GetOptionalInt(row, 1, "NR_TURNO", "TURNO"),
                zona,
                secao,
                GetRequiredInt(row, "QT_VOTOS", "QT_VOTOS_NOMINAIS", "QUANTIDADE_VOTOS"));
        }).ToList();
    }

    public async Task<IReadOnlyList<PerfilEleitorado>> ParseEleitoradoAsync(
        TextReader reader,
        AnoEleicao anoEleicao,
        CancellationToken ct = default)
    {
        var rows = await ReadRowsAsync(reader, ct);
        return rows.Select(row =>
        {
            var uf = new UF(GetRequiredString(row, "SG_UF", "UF"));
            var municipio = new Municipio(
                GetRequiredInt(row, "CD_MUNICIPIO", "CODIGO_IBGE", "COD_MUNICIPIO"),
                GetRequiredString(row, "NM_MUNICIPIO", "MUNICIPIO"),
                uf);
            var zona = new ZonaEleitoral(GetRequiredInt(row, "NR_ZONA", "ZONA"), municipio);

            return new PerfilEleitorado(
                anoEleicao,
                zona,
                GetRequiredString(row, "DS_FAIXA_ETARIA", "FAIXA_ETARIA"),
                GetRequiredString(row, "DS_GRAU_ESCOLARIDADE", "ESCOLARIDADE"),
                GetRequiredString(row, "DS_GENERO", "GENERO"),
                GetRequiredInt(row, "QT_ELEITORES_PERFIL", "QUANTIDADE_ELEITORES"));
        }).ToList();
    }

    public async Task<IReadOnlyList<BemDeclarado>> ParseBensDeclaradosAsync(
        TextReader reader,
        AnoEleicao anoEleicao,
        CancellationToken ct = default)
    {
        var rows = await ReadRowsAsync(reader, ct);
        return rows.Select(row =>
        {
            var candidato = ParseCandidato(row, anoEleicao);
            return new BemDeclarado(
                candidato,
                anoEleicao,
                GetRequiredString(row, "DS_TIPO_BEM_CANDIDATO", "TIPO_BEM"),
                GetRequiredString(row, "DS_BEM_CANDIDATO", "DESCRICAO"),
                GetRequiredDecimal(row, "VR_BEM_CANDIDATO", "VALOR"));
        }).ToList();
    }

    public async Task<IReadOnlyList<PrestacaoContas>> ParsePrestacaoContasAsync(
        TextReader reader,
        AnoEleicao anoEleicao,
        CancellationToken ct = default)
    {
        var rows = await ReadRowsAsync(reader, ct);
        return rows.Select(row =>
        {
            var candidato = ParseCandidato(row, anoEleicao);
            return new PrestacaoContas(
                candidato,
                anoEleicao,
                GetRequiredString(row, "DS_TIPO_RECEITA", "TIPO_RECEITA"),
                GetRequiredString(row, "DS_RECEITA", "DESCRICAO"),
                GetRequiredDecimal(row, "VR_RECEITA", "VALOR"),
                GetRequiredString(row, "DS_TIPO_MOVIMENTO", "TIPO_MOVIMENTACAO"));
        }).ToList();
    }

    public async Task<IReadOnlyList<Coligacao>> ParseColigacoesAsync(
        TextReader reader,
        AnoEleicao anoEleicao,
        CancellationToken ct = default)
    {
        var rows = await ReadRowsAsync(reader, ct);
        var coligacoes = new Dictionary<string, List<Partido>>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in rows)
        {
            var nome = GetRequiredString(row, "NM_COLIGACAO", "COLIGACAO");
            var partido = new Partido(
                GetRequiredInt(row, "NR_PARTIDO", "NUMERO_PARTIDO"),
                GetRequiredString(row, "SG_PARTIDO"),
                GetRequiredString(row, "NM_PARTIDO"));

            if (!coligacoes.TryGetValue(nome, out var partidos))
            {
                partidos = [];
                coligacoes[nome] = partidos;
            }

            if (!partidos.Any(x => x.Numero == partido.Numero && x.Sigla == partido.Sigla && x.Nome == partido.Nome))
            {
                partidos.Add(partido);
            }
        }

        return coligacoes
            .Select(item => new Coligacao(item.Key, anoEleicao, item.Value))
            .ToList();
    }

    private static Candidato ParseCandidato(CsvRow row, AnoEleicao anoEleicao)
    {
        var uf = new UF(GetRequiredString(row, "SG_UF", "UF"));
        return new Candidato(
            GetRequiredString(row, "NR_CPF_CANDIDATO", "CPF_CANDIDATO", "CPF"),
            GetRequiredString(row, "NM_CANDIDATO", "NOME_CANDIDATO"),
            GetRequiredString(row, "NM_URNA_CANDIDATO", "NOME_URNA_CANDIDATO", "NM_URNA"),
            GetRequiredInt(row, "NR_CANDIDATO", "NUMERO_CANDIDATO"),
            GetRequiredString(row, "DS_CARGO", "CARGO"),
            new Partido(
                GetRequiredInt(row, "NR_PARTIDO", "NUMERO_PARTIDO"),
                GetRequiredString(row, "SG_PARTIDO"),
                GetRequiredString(row, "NM_PARTIDO")),
            anoEleicao,
            uf);
    }

    private static async Task<IReadOnlyList<CsvRow>> ReadRowsAsync(TextReader reader, CancellationToken ct)
    {
        var firstLine = await reader.ReadLineAsync(ct);
        if (string.IsNullOrWhiteSpace(firstLine))
            return [];

        var delimiter = DetectDelimiter(firstLine);
        var headers = ParseLine(firstLine, delimiter).Select(NormalizeHeader).ToArray();
        var rows = new List<CsvRow>();

        string? line;
        while ((line = await reader.ReadLineAsync(ct)) is not null)
        {
            ct.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var values = ParseLine(line, delimiter);
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (var index = 0; index < headers.Length; index++)
            {
                dict[headers[index]] = index < values.Count ? values[index].Trim() : string.Empty;
            }

            rows.Add(new CsvRow(dict));
        }

        return rows;
    }

    private static char DetectDelimiter(string line)
        => line.Count(ch => ch == ';') >= line.Count(ch => ch == ',') ? ';' : ',';

    private static IReadOnlyList<string> ParseLine(string line, char delimiter)
    {
        var values = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        for (var index = 0; index < line.Length; index++)
        {
            var currentChar = line[index];

            if (currentChar == '"')
            {
                if (inQuotes && index + 1 < line.Length && line[index + 1] == '"')
                {
                    current.Append('"');
                    index++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }

                continue;
            }

            if (currentChar == delimiter && !inQuotes)
            {
                values.Add(current.ToString());
                current.Clear();
                continue;
            }

            current.Append(currentChar);
        }

        values.Add(current.ToString());
        return values;
    }

    private static string GetRequiredString(CsvRow row, params string[] columnNames)
    {
        var value = GetOptionalString(row, columnNames);
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Coluna obrigatória ausente: {string.Join(", ", columnNames)}.");

        return value;
    }

    private static string? GetOptionalString(CsvRow row, params string[] columnNames)
    {
        foreach (var columnName in columnNames)
        {
            if (row.Values.TryGetValue(NormalizeHeader(columnName), out var value) && !string.IsNullOrWhiteSpace(value))
                return value;
        }

        return null;
    }

    private static int GetRequiredInt(CsvRow row, params string[] columnNames)
    {
        var value = GetRequiredString(row, columnNames);
        return ParseInt(value, columnNames);
    }

    private static int GetOptionalInt(CsvRow row, int defaultValue, params string[] columnNames)
    {
        var value = GetOptionalString(row, columnNames);
        return value is null ? defaultValue : ParseInt(value, columnNames);
    }

    private static decimal GetRequiredDecimal(CsvRow row, params string[] columnNames)
    {
        var value = GetRequiredString(row, columnNames);
        return ParseDecimal(value, columnNames);
    }

    private static int ParseInt(string value, params string[] columnNames)
    {
        var sanitized = value.Replace(".", string.Empty).Replace(" ", string.Empty);
        if (!int.TryParse(sanitized, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
            throw new FormatException($"Valor inteiro inválido '{value}' para as colunas {string.Join(", ", columnNames)}.");

        return parsed;
    }

    private static decimal ParseDecimal(string value, params string[] columnNames)
    {
        var sanitized = value.Trim();
        if (decimal.TryParse(sanitized, NumberStyles.Number, new CultureInfo("pt-BR"), out var parsed) ||
            decimal.TryParse(sanitized, NumberStyles.Number, CultureInfo.InvariantCulture, out parsed))
        {
            return parsed;
        }

        throw new FormatException($"Valor decimal inválido '{value}' para as colunas {string.Join(", ", columnNames)}.");
    }

    private static string NormalizeHeader(string value)
    {
        var normalized = value.Trim().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var current in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(current);
            if (category == UnicodeCategory.NonSpacingMark)
                continue;

            if (char.IsLetterOrDigit(current))
                builder.Append(char.ToUpperInvariant(current));
        }

        return builder.ToString();
    }

    private sealed record CsvRow(IReadOnlyDictionary<string, string> Values);
}
