using System.Text;
using Data;
using NetTopologySuite.Geometries;

namespace Etl.Tests.Ibge;

public sealed class IbgeSpatialGeoJsonParserTests
{
    [Fact]
    public void ParseSetoresCensitarios_ConvertePolygonEmMultiPolygon()
    {
        var parser = new IbgeSpatialGeoJsonParser();

        var setores = parser.ParseSetoresCensitarios(ToStream("""
            {
              "type": "FeatureCollection",
              "features": [
                {
                  "type": "Feature",
                  "properties": {
                    "cd_setor": "355030800000001",
                    "cd_mun": 3550308,
                    "nm_mun": "Sao Paulo",
                    "uf": "SP"
                  },
                  "geometry": {
                    "type": "Polygon",
                    "coordinates": [[
                      [-46.65, -23.55],
                      [-46.64, -23.55],
                      [-46.64, -23.54],
                      [-46.65, -23.54],
                      [-46.65, -23.55]
                    ]]
                  }
                }
              ]
            }
            """));

        var setor = Assert.Single(setores);
        Assert.IsType<MultiPolygon>(setor.Geometria);
        Assert.Equal(4674, setor.Geometria.SRID);
        Assert.Equal("355030800000001", setor.CodigoSetor);
        Assert.Equal(3550308, setor.MunicipioCodigoIbge);
        Assert.Equal("Sao Paulo", setor.MunicipioNome);
        Assert.Equal("SP", setor.UfSigla);
    }

    [Fact]
    public void ParseSetoresCensitarios_ComTipoNaoSuportado_Falha()
    {
        var parser = new IbgeSpatialGeoJsonParser();

        var ex = Assert.Throws<InvalidOperationException>(() => parser.ParseSetoresCensitarios(ToStream("""
            {
              "type": "Feature",
              "properties": {
                "cd_setor": "355030800000001",
                "cd_mun": 3550308,
                "nm_mun": "Sao Paulo",
                "uf": "SP"
              },
              "geometry": {
                "type": "Point",
                "coordinates": [-46.65, -23.55]
              }
            }
            """)));

        Assert.Contains("nao suportado", ex.Message);
    }

    private static MemoryStream ToStream(string geoJson)
        => new(Encoding.UTF8.GetBytes(geoJson));
}
