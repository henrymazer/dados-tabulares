using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;

namespace Etl.Ibge;

public sealed class IbgeAggregateStagingPipeline(PublicDataDbContext context)
{
    public async Task<int> IngerirAsync(string zipPath, CancellationToken ct = default)
    {
        var packageName = Path.GetFileNameWithoutExtension(zipPath).ToLowerInvariant();
        using var archive = ZipFile.OpenRead(zipPath);
        var csvEntry = archive.Entries.Single(entry => entry.FullName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase));

        using var entryStream = csvEntry.Open();
        using var reader = new StreamReader(entryStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        var headerLine = reader.ReadLine() ?? throw new InvalidOperationException("Pacote agregado IBGE sem cabeçalho.");
        var delimiter = DetectDelimiter(headerLine);
        var headers = ParseHeaderLine(headerLine, delimiter);

        using var parser = new TextFieldParser(reader)
        {
            TextFieldType = FieldType.Delimited,
            HasFieldsEnclosedInQuotes = true,
            TrimWhiteSpace = true
        };

        parser.SetDelimiters(delimiter.ToString());
        var rows = new List<IbgeAgregadoStagingRecord>();

        while (!parser.EndOfData)
        {
            var fields = parser.ReadFields();
            if (fields is null || fields.All(string.IsNullOrWhiteSpace))
            {
                continue;
            }

            var payload = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var index = 0; index < headers.Length && index < fields.Length; index++)
            {
                payload[headers[index]] = fields[index].Trim();
            }

            var codigoSetor = payload.FirstOrDefault(pair => pair.Key.Equals("CD_SETOR", StringComparison.OrdinalIgnoreCase) || pair.Key.Equals("CD_setor", StringComparison.OrdinalIgnoreCase)).Value;
            if (string.IsNullOrWhiteSpace(codigoSetor))
            {
                throw new InvalidOperationException("Pacote agregado IBGE sem coluna CD_SETOR/CD_setor.");
            }

            rows.Add(new IbgeAgregadoStagingRecord
            {
                Pacote = packageName,
                NomeArquivoInterno = csvEntry.FullName,
                CodigoSetor = codigoSetor,
                PayloadJson = JsonSerializer.Serialize(payload)
            });
        }

        await using var transaction = await context.Database.BeginTransactionAsync(ct);
        await context.IbgeAgregadosStaging.Where(x => x.Pacote == packageName).ExecuteDeleteAsync(ct);
        context.ChangeTracker.Clear();
        await context.IbgeAgregadosStaging.AddRangeAsync(rows, ct);
        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return rows.Count;
    }

    private static char DetectDelimiter(string headerLine)
        => headerLine.Count(c => c == ';') >= headerLine.Count(c => c == ',') ? ';' : ',';

    private static string[] ParseHeaderLine(string headerLine, char delimiter)
        => headerLine
            .Split(delimiter)
            .Select(column => column.Trim().Trim('"'))
            .ToArray();
}
