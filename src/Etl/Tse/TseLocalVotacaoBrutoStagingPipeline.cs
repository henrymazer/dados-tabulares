using System.IO.Compression;
using System.Text.Json;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;

namespace DadosTabulares.Etl.Tse;

public sealed class TseLocalVotacaoBrutoStagingPipeline(PublicDataDbContext context)
{
    public async Task<int> IngerirAsync(string zipPath, int anoEleicao, CancellationToken ct = default)
    {
        using var archive = ZipFile.OpenRead(zipPath);
        var csvEntries = archive.Entries.Where(entry => entry.FullName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase)).ToList();
        var rows = new List<TseLocalVotacaoBrutoStagingRecord>();

        foreach (var csvEntry in csvEntries)
        {
            using var entryStream = csvEntry.Open();
            using var parser = new TextFieldParser(entryStream)
            {
                TextFieldType = FieldType.Delimited,
                HasFieldsEnclosedInQuotes = true,
                TrimWhiteSpace = true
            };

            parser.SetDelimiters(";");

            var headers = parser.ReadFields() ?? throw new InvalidOperationException("Arquivo TSE sem cabeçalho.");
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

                rows.Add(new TseLocalVotacaoBrutoStagingRecord
                {
                    AnoEleicao = anoEleicao,
                    UfSigla = payload.GetRequired("SG_UF"),
                    CodigoUnidadeEleitoral = payload.GetRequired("SG_UE"),
                    NomeUnidadeEleitoral = payload.GetRequired("NM_UE"),
                    MunicipioCodigoIbge = payload.GetRequired("CD_MUNICIPIO"),
                    MunicipioNome = payload.GetRequired("NM_MUNICIPIO"),
                    NumeroZona = int.Parse(payload.GetRequired("NR_ZONA")),
                    NumeroSecao = int.Parse(payload.GetRequired("NR_SECAO")),
                    NumeroLocalVotacao = int.Parse(payload.GetRequired("NR_LOCAL_VOTACAO")),
                    NomeLocalVotacao = payload.GetRequired("NM_LOCAL_VOTACAO"),
                    EnderecoLocalVotacao = payload.GetRequired("DS_LOCAL_VOTACAO_ENDERECO"),
                    PayloadJson = JsonSerializer.Serialize(payload)
                });
            }
        }

        await using var transaction = await context.Database.BeginTransactionAsync(ct);
        await context.TseLocaisVotacaoBrutos.Where(x => x.AnoEleicao == anoEleicao).ExecuteDeleteAsync(ct);
        context.ChangeTracker.Clear();
        await context.TseLocaisVotacaoBrutos.AddRangeAsync(rows, ct);
        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return rows.Count;
    }
}

internal static class TseLocalVotacaoPayloadExtensions
{
    public static string GetRequired(this IReadOnlyDictionary<string, string> payload, string key)
        => payload.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new InvalidOperationException($"Campo obrigatório do staging TSE ausente: {key}.");
}
