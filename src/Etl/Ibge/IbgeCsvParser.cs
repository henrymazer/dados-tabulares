using System.Globalization;
using DadosTabulares.Domain.Ibge;
using DadosTabulares.Domain.Tse;
using Microsoft.VisualBasic.FileIO;

namespace Etl.Ibge;

public sealed class IbgeCsvParser
{
    public IReadOnlyList<DadoPopulacional> ParsePopulacional(Stream csv)
        => Parse(csv, row => new DadoPopulacional(
            ToMunicipio(row, "codigo_ibge", "municipio", "uf"),
            row.GetRequired("faixa_etaria"),
            row.GetRequired("raca"),
            row.GetIntRequired("quantidade")));

    public IReadOnlyList<DadoEscolaridade> ParseEscolaridade(Stream csv)
        => Parse(csv, row => new DadoEscolaridade(
            ToMunicipio(row, "codigo_ibge", "municipio", "uf"),
            row.GetRequired("nivel_escolaridade"),
            row.GetIntRequired("quantidade")));

    public IReadOnlyList<DadoRenda> ParseRenda(Stream csv)
        => Parse(csv, row => new DadoRenda(
            ToMunicipio(row, "codigo_ibge", "municipio", "uf"),
            row.GetRequired("faixa_renda"),
            row.GetIntRequired("quantidade")));

    public IReadOnlyList<DadoSaneamento> ParseSaneamento(Stream csv)
        => Parse(csv, row => new DadoSaneamento(
            ToMunicipio(row, "codigo_ibge", "municipio", "uf"),
            row.GetRequired("tipo_saneamento"),
            row.GetIntRequired("domicilios_atendidos")));

    public IReadOnlyList<DadoUrbanizacao> ParseUrbanizacao(Stream csv)
        => Parse(csv, row => new DadoUrbanizacao(
            ToMunicipio(row, "codigo_ibge", "municipio", "uf"),
            row.GetRequired("tipo_area"),
            row.GetIntRequired("populacao")));

    public IReadOnlyList<DadoInfraestrutura> ParseInfraestrutura(Stream csv)
        => Parse(csv, row => new DadoInfraestrutura(
            ToMunicipio(row, "codigo_ibge", "municipio", "uf"),
            row.GetRequired("tipo_infraestrutura"),
            row.GetIntRequired("domicilios_atendidos")));

    private static IReadOnlyList<T> Parse<T>(Stream csv, Func<ParsedRow, T> map)
    {
        using var parser = new TextFieldParser(csv)
        {
            TextFieldType = FieldType.Delimited,
            HasFieldsEnclosedInQuotes = true,
            TrimWhiteSpace = true
        };

        parser.SetDelimiters(",", ";");

        var headers = parser.ReadFields() ?? throw new InvalidOperationException("Arquivo IBGE sem cabeçalho.");
        var normalizedHeaders = headers.Select(NormalizeHeader).ToArray();

        var items = new List<T>();
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

            items.Add(map(new ParsedRow(values)));
        }

        return items;
    }

    private static Municipio ToMunicipio(ParsedRow row, string codigoHeader, string municipioHeader, string ufHeader)
        => new(
            row.GetIntRequired(codigoHeader),
            row.GetRequired(municipioHeader),
            new UF(row.GetRequired(ufHeader)));

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

    private sealed class ParsedRow(IReadOnlyDictionary<string, string> values)
    {
        public string GetRequired(string header)
        {
            if (values.TryGetValue(NormalizeHeader(header), out var value) && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            throw new InvalidOperationException($"Campo IBGE obrigatório ausente: {header}.");
        }

        public int GetIntRequired(string header)
        {
            var value = GetRequired(header);
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
            {
                return parsed;
            }

            throw new InvalidOperationException($"Campo IBGE '{header}' precisa ser inteiro, mas recebeu '{value}'.");
        }
    }
}
