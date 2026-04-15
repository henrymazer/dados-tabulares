using Data;
using Microsoft.Data.Sqlite;
using NetTopologySuite.Geometries;

namespace Etl.Ibge;

public sealed class IbgeGeoPackageReader
{
    public IReadOnlyList<MunicipioMalhaRecord> ReadMunicipios(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        using var connection = Open(filePath);
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT geom, CD_MUN, NM_MUN, CD_RGI, NM_RGI, CD_RGINT, NM_RGINT, CD_UF, NM_UF, SIGLA_UF, CD_REGIAO, NM_REGIAO, SIGLA_RG, CD_CONCURB, NM_CONCURB, AREA_KM2
            FROM br_municipios_2025shp
            """;

        using var reader = command.ExecuteReader();
        var geomOrdinal = reader.GetOrdinal("geom");
        var items = new List<MunicipioMalhaRecord>();
        while (reader.Read())
        {
            items.Add(new MunicipioMalhaRecord
            {
                CodigoMunicipio = reader.GetString(1),
                NomeMunicipio = GetString(reader, 2),
                CodigoRegiaoImediata = GetString(reader, 3),
                NomeRegiaoImediata = GetString(reader, 4),
                CodigoRegiaoIntermediaria = GetString(reader, 5),
                NomeRegiaoIntermediaria = GetString(reader, 6),
                CodigoUf = GetString(reader, 7),
                NomeUf = GetString(reader, 8),
                UfSigla = GetString(reader, 9),
                CodigoRegiao = GetString(reader, 10),
                NomeRegiao = GetString(reader, 11),
                SiglaRegiao = GetString(reader, 12),
                CodigoConcentracaoUrbana = GetString(reader, 13),
                NomeConcentracaoUrbana = GetString(reader, 14),
                AreaKm2 = GetDouble(reader, 15),
                Geometria = ReadRequiredGeometry(reader, geomOrdinal)
            });
        }

        return items;
    }

    public IEnumerable<SetorCensitarioRecord> StreamSetores(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        using var connection = Open(filePath);
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT geom, CD_SETOR, SITUACAO, CD_SIT, CD_TIPO, AREA_KM2, CD_REGIAO, NM_REGIAO, CD_UF, NM_UF, CD_MUN, NM_MUN,
                   CD_DIST, NM_DIST, CD_SUBDIST, NM_SUBDIST, CD_BAIRRO, NM_BAIRRO, CD_NU, NM_NU, CD_FCU, NM_FCU, CD_AGLOM, NM_AGLOM,
                   CD_RGINT, NM_RGINT, CD_RGI, NM_RGI, CD_CONCURB, NM_CONCURB
            FROM BR_setores_CD2022
            ORDER BY CD_SETOR
            """;

        using var reader = command.ExecuteReader();
        var geomOrdinal = reader.GetOrdinal("geom");
        SetorCensitarioRecord? current = null;
        while (reader.Read())
        {
            var setor = CreateSetor(reader, geomOrdinal);
            if (current is null)
            {
                current = setor;
                continue;
            }

            if (StringComparer.Ordinal.Equals(current.CodigoSetor, setor.CodigoSetor))
            {
                current.Geometria = MergeMultiPolygon(current.Geometria, setor.Geometria);
                continue;
            }

            yield return current;
            current = setor;
        }

        if (current is not null)
        {
            yield return current;
        }
    }

    public IReadOnlyList<SetorCensitarioRecord> ReadSetores(string filePath)
        => StreamSetores(filePath).ToList();

    private static SetorCensitarioRecord CreateSetor(SqliteDataReader reader, int geomOrdinal)
    {
        var geometria = ReadRequiredGeometry(reader, geomOrdinal);
        return new SetorCensitarioRecord
        {
            CodigoSetor = reader.GetString(1),
            Situacao = GetString(reader, 2),
            CodigoSituacao = GetString(reader, 3),
            CodigoTipo = GetString(reader, 4),
            AreaKm2 = GetDouble(reader, 5),
            CodigoRegiao = GetString(reader, 6),
            NomeRegiao = GetString(reader, 7),
            CodigoUf = GetString(reader, 8),
            NomeUf = GetString(reader, 9),
            MunicipioCodigoIbge = GetString(reader, 10),
            MunicipioNome = GetString(reader, 11),
            CodigoDistrito = GetString(reader, 12),
            NomeDistrito = GetString(reader, 13),
            CodigoSubdistrito = GetString(reader, 14),
            NomeSubdistrito = GetString(reader, 15),
            CodigoBairro = GetString(reader, 16),
            NomeBairro = GetString(reader, 17),
            CodigoNucleoUrbano = GetString(reader, 18),
            NomeNucleoUrbano = GetString(reader, 19),
            CodigoFcu = GetString(reader, 20),
            NomeFcu = GetString(reader, 21),
            CodigoAglomerado = GetString(reader, 22),
            NomeAglomerado = GetString(reader, 23),
            CodigoRegiaoIntermediaria = GetString(reader, 24),
            NomeRegiaoIntermediaria = GetString(reader, 25),
            CodigoRegiaoImediata = GetString(reader, 26),
            NomeRegiaoImediata = GetString(reader, 27),
            CodigoConcentracaoUrbana = GetString(reader, 28),
            NomeConcentracaoUrbana = GetString(reader, 29),
            UfSigla = GetUfSigla(GetString(reader, 8)),
            Geometria = geometria
        };
    }

    private static SqliteConnection Open(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("GeoPackage não encontrado.", filePath);
        }

        var connection = new SqliteConnection(new SqliteConnectionStringBuilder
        {
            DataSource = filePath,
            Mode = SqliteOpenMode.ReadOnly
        }.ToString());
        connection.Open();
        return connection;
    }

    private static string GetString(SqliteDataReader reader, int ordinal)
        => reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);

    private static double GetDouble(SqliteDataReader reader, int ordinal)
        => reader.IsDBNull(ordinal) ? 0d : reader.GetDouble(ordinal);

    private static MultiPolygon ReadRequiredGeometry(SqliteDataReader reader, int ordinal)
    {
        if (reader.IsDBNull(ordinal))
        {
            throw new InvalidOperationException("Coluna geom obrigatória está ausente no GeoPackage.");
        }

        return GeoPackageGeometryReader.ReadMultiPolygon((byte[])reader.GetValue(ordinal));
    }

    private static string GetUfSigla(string codigoUf)
        => codigoUf switch
        {
            "11" => "RO",
            "12" => "AC",
            "13" => "AM",
            "14" => "RR",
            "15" => "PA",
            "16" => "AP",
            "17" => "TO",
            "21" => "MA",
            "22" => "PI",
            "23" => "CE",
            "24" => "RN",
            "25" => "PB",
            "26" => "PE",
            "27" => "AL",
            "28" => "SE",
            "29" => "BA",
            "31" => "MG",
            "32" => "ES",
            "33" => "RJ",
            "35" => "SP",
            "41" => "PR",
            "42" => "SC",
            "43" => "RS",
            "50" => "MS",
            "51" => "MT",
            "52" => "GO",
            "53" => "DF",
            _ => string.Empty
        };

    private static MultiPolygon MergeMultiPolygon(MultiPolygon first, MultiPolygon second)
    {
        var polygons = new List<Polygon>(first.NumGeometries + second.NumGeometries);

        for (var index = 0; index < first.NumGeometries; index++)
        {
            polygons.Add((Polygon)first.GetGeometryN(index));
        }

        for (var index = 0; index < second.NumGeometries; index++)
        {
            polygons.Add((Polygon)second.GetGeometryN(index));
        }

        return first.Factory.CreateMultiPolygon(polygons.ToArray());
    }
}
