using System.Globalization;
using System.Text.Json;
using Data;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace Etl.Ibge;

public sealed class IbgeSpatialGeoJsonParser
{
    private static readonly GeometryFactory GeometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4674);

    public IReadOnlyList<SetorCensitarioRecord> ParseSetoresCensitarios(Stream geoJson)
    {
        ArgumentNullException.ThrowIfNull(geoJson);

        using var document = JsonDocument.Parse(geoJson);
        var root = document.RootElement;
        var rootType = root.GetProperty("type").GetString();

        return rootType switch
        {
            "FeatureCollection" => ParseFeatureCollection(root),
            "Feature" => [ParseFeature(root)],
            _ => throw new InvalidOperationException("GeoJSON IBGE invalido: tipo raiz nao suportado.")
        };
    }

    private static IReadOnlyList<SetorCensitarioRecord> ParseFeatureCollection(JsonElement root)
    {
        var features = root.GetProperty("features");
        var items = new List<SetorCensitarioRecord>(features.GetArrayLength());

        foreach (var feature in features.EnumerateArray())
        {
            items.Add(ParseFeature(feature));
        }

        return items;
    }

    private static SetorCensitarioRecord ParseFeature(JsonElement feature)
    {
        var properties = ReadProperties(feature.GetProperty("properties"));
        var geometry = ParseGeometry(feature.GetProperty("geometry"));

        return new SetorCensitarioRecord
        {
            CodigoSetor = GetRequired(properties, "codigo_setor", "cd_setor", "cod_setor", "setor", "id"),
            MunicipioCodigoIbge = GetRequiredInt(properties, "municipio_codigo_ibge", "codigo_ibge", "cd_mun", "cod_municipio", "cd_municipio"),
            MunicipioNome = GetRequired(properties, "municipio_nome", "municipio", "nm_mun", "nome_municipio"),
            UfSigla = GetRequired(properties, "uf_sigla", "uf", "sigla_uf").ToUpperInvariant(),
            Geometria = geometry
        };
    }

    private static Dictionary<string, string> ReadProperties(JsonElement properties)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var property in properties.EnumerateObject())
        {
            values[NormalizeKey(property.Name)] = property.Value.ValueKind switch
            {
                JsonValueKind.String => property.Value.GetString() ?? string.Empty,
                JsonValueKind.Number => property.Value.GetRawText(),
                _ => property.Value.GetRawText()
            };
        }

        return values;
    }

    private static MultiPolygon ParseGeometry(JsonElement geometry)
    {
        var geometryType = geometry.GetProperty("type").GetString();

        var parsed = geometryType switch
        {
            "Polygon" => GeometryFactory.CreateMultiPolygon([ParsePolygon(geometry.GetProperty("coordinates"))]),
            "MultiPolygon" => GeometryFactory.CreateMultiPolygon(ParseMultiPolygon(geometry.GetProperty("coordinates"))),
            _ => throw new InvalidOperationException($"Tipo de geometria IBGE nao suportado: '{geometryType}'.")
        };

        if (parsed.IsEmpty)
        {
            throw new InvalidOperationException("Geometria IBGE invalida: geometria vazia.");
        }

        if (!parsed.IsValid)
        {
            throw new InvalidOperationException("Geometria IBGE invalida: geometria mal formada.");
        }

        return parsed;
    }

    private static Polygon ParsePolygon(JsonElement coordinates)
    {
        var rings = coordinates.EnumerateArray().Select(ParseLinearRing).ToArray();
        if (rings.Length == 0)
        {
            throw new InvalidOperationException("Geometria IBGE invalida: poligono sem anel.");
        }

        return GeometryFactory.CreatePolygon(rings[0], rings.Skip(1).ToArray());
    }

    private static Polygon[] ParseMultiPolygon(JsonElement coordinates)
        => coordinates.EnumerateArray().Select(ParsePolygon).ToArray();

    private static LinearRing ParseLinearRing(JsonElement ring)
    {
        var points = ring.EnumerateArray()
            .Select(ParseCoordinate)
            .ToList();

        if (points.Count < 4)
        {
            throw new InvalidOperationException("Geometria IBGE invalida: anel com menos de quatro pontos.");
        }

        if (!points[0].Equals2D(points[^1]))
        {
            points.Add(new Coordinate(points[0]));
        }

        return GeometryFactory.CreateLinearRing(points.ToArray());
    }

    private static Coordinate ParseCoordinate(JsonElement point)
    {
        var values = point.EnumerateArray().ToArray();
        if (values.Length < 2)
        {
            throw new InvalidOperationException("Geometria IBGE invalida: coordenada incompleta.");
        }

        return new Coordinate(values[0].GetDouble(), values[1].GetDouble());
    }

    private static string GetRequired(IReadOnlyDictionary<string, string> properties, params string[] aliases)
    {
        foreach (var alias in aliases.Select(NormalizeKey))
        {
            if (properties.TryGetValue(alias, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }
        }

        throw new InvalidOperationException($"Propriedade IBGE obrigatoria ausente: {aliases[0]}.");
    }

    private static int GetRequiredInt(IReadOnlyDictionary<string, string> properties, params string[] aliases)
    {
        var value = GetRequired(properties, aliases);
        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
        {
            return parsed;
        }

        throw new InvalidOperationException($"Propriedade IBGE precisa ser inteira: {aliases[0]}.");
    }

    private static string NormalizeKey(string value)
        => new(value.Trim().ToLowerInvariant()
            .Where(c => c is not '_' and not '-' and not ' ')
            .ToArray());
}
