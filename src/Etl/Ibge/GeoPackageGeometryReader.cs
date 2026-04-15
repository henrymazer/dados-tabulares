using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Etl.Ibge;

internal static class GeoPackageGeometryReader
{
    public static MultiPolygon ReadMultiPolygon(byte[] blob)
    {
        ArgumentNullException.ThrowIfNull(blob);

        if (blob.Length < 8 || blob[0] != 0x47 || blob[1] != 0x50)
        {
            throw new InvalidOperationException("Blob GeoPackage inválido.");
        }

        var flags = blob[3];
        var littleEndian = (flags & 0x01) == 1;
        var envelopeIndicator = (flags >> 1) & 0x07;
        var envelopeBytes = envelopeIndicator switch
        {
            0 => 0,
            1 => 32,
            2 => 48,
            3 => 48,
            4 => 64,
            _ => throw new InvalidOperationException($"Envelope GeoPackage não suportado: {envelopeIndicator}.")
        };

        var srid = ReadInt32(blob, 4, littleEndian);
        var wkbOffset = 8 + envelopeBytes;
        var geometry = new WKBReader().Read(blob[wkbOffset..]);
        geometry.SRID = srid;

        return geometry switch
        {
            MultiPolygon multiPolygon => multiPolygon,
            Polygon polygon => polygon.Factory.CreateMultiPolygon([polygon]),
            _ => throw new InvalidOperationException($"Geometria GeoPackage não suportada: {geometry.GeometryType}.")
        };
    }

    private static int ReadInt32(byte[] bytes, int offset, bool littleEndian)
    {
        Span<byte> buffer = stackalloc byte[4];
        bytes.AsSpan(offset, 4).CopyTo(buffer);

        if (BitConverter.IsLittleEndian != littleEndian)
        {
            buffer.Reverse();
        }

        return BitConverter.ToInt32(buffer);
    }
}
