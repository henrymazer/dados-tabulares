using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Etl;

public sealed class CargaBrutaSnapshotService(PublicDataDbContext context)
{
    public async Task<CargaBrutaSnapshotPreparado> PrepararAsync(EtlCommand command, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var absolutePath = Path.GetFullPath(command.FilePath);
        var fonte = command.Kind.ToString().ToLowerInvariant();
        var dataset = (string.IsNullOrWhiteSpace(command.Dataset) ? command.Source : command.Dataset).ToLowerInvariant();
        var escopo = command.Year?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        var (nomeArquivoOriginal, tamanhoBytes, hashSnapshot) = ResolveSnapshotSource(absolutePath);
        var snapshotExistente = await context.CargasBrutasSnapshots
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.Fonte == fonte &&
                     x.Dataset == dataset &&
                     x.Escopo == escopo &&
                     x.HashSnapshot == hashSnapshot &&
                     x.IsCurrent,
                ct);

        return new CargaBrutaSnapshotPreparado(
            DeveIngerir: snapshotExistente is null,
            Fonte: fonte,
            Dataset: dataset,
            Escopo: escopo,
            ChaveSnapshot: Path.GetFileNameWithoutExtension(absolutePath),
            HashSnapshot: hashSnapshot,
            NomeArquivoOriginal: nomeArquivoOriginal,
            CaminhoArquivoOriginal: absolutePath,
            TamanhoBytes: tamanhoBytes);
    }

    public async Task RegistrarCargaAplicadaAsync(CargaBrutaSnapshotPreparado snapshot, int registrosImportados, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var agora = DateTimeOffset.UtcNow;
        var snapshotsAtuais = await context.CargasBrutasSnapshots
            .Where(x => x.Fonte == snapshot.Fonte &&
                        x.Dataset == snapshot.Dataset &&
                        x.Escopo == snapshot.Escopo &&
                        x.IsCurrent)
            .ToListAsync(ct);

        foreach (var atual in snapshotsAtuais)
        {
            atual.IsCurrent = false;
            atual.SubstituidoEmUtc = agora;
        }

        var novoSnapshot = new CargaBrutaSnapshotRecord
        {
            Fonte = snapshot.Fonte,
            Dataset = snapshot.Dataset,
            Escopo = snapshot.Escopo,
            ChaveSnapshot = snapshot.ChaveSnapshot,
            HashSnapshot = snapshot.HashSnapshot,
            NomeArquivoOriginal = snapshot.NomeArquivoOriginal,
            CaminhoArquivoOriginal = snapshot.CaminhoArquivoOriginal,
            TamanhoBytes = snapshot.TamanhoBytes,
            RegistrosImportados = registrosImportados,
            IsCurrent = true,
            RegistradoEmUtc = agora
        };

        await context.CargasBrutasSnapshots.AddAsync(novoSnapshot, ct);
        await context.SaveChangesAsync(ct);

        await context.CargasBrutasAuditorias.AddAsync(new CargaBrutaAuditoriaRecord
        {
            SnapshotId = novoSnapshot.Id,
            Fonte = snapshot.Fonte,
            Dataset = snapshot.Dataset,
            Escopo = snapshot.Escopo,
            ChaveSnapshot = snapshot.ChaveSnapshot,
            HashSnapshot = snapshot.HashSnapshot,
            NomeArquivoOriginal = snapshot.NomeArquivoOriginal,
            CaminhoArquivoOriginal = snapshot.CaminhoArquivoOriginal,
            TamanhoBytes = snapshot.TamanhoBytes,
            Status = "applied",
            RegistrosImportados = registrosImportados,
            RegistradoEmUtc = agora
        }, ct);

        await context.SaveChangesAsync(ct);
    }

    private static string ComputeSha256(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        var hash = SHA256.HashData(stream);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static (string NomeArquivoOriginal, long TamanhoBytes, string HashSnapshot) ResolveSnapshotSource(string path)
    {
        if (File.Exists(path))
        {
            var fileInfo = new FileInfo(path);
            return (fileInfo.Name, fileInfo.Length, ComputeSha256(path));
        }

        if (Directory.Exists(path))
        {
            var files = Directory.EnumerateFiles(path, "*.csv", SearchOption.TopDirectoryOnly)
                .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (files.Count == 0)
                throw new FileNotFoundException("Diretório bruto sem arquivos CSV para snapshot.", path);

            using var sha256 = SHA256.Create();
            long totalBytes = 0;
            foreach (var file in files)
            {
                var fileNameBytes = Encoding.UTF8.GetBytes(Path.GetFileName(file));
                sha256.TransformBlock(fileNameBytes, 0, fileNameBytes.Length, null, 0);

                var fileBytes = File.ReadAllBytes(file);
                totalBytes += fileBytes.LongLength;
                sha256.TransformBlock(fileBytes, 0, fileBytes.Length, null, 0);
            }

            sha256.TransformFinalBlock([], 0, 0);
            return (Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar)), totalBytes, Convert.ToHexString(sha256.Hash!).ToLowerInvariant());
        }

        throw new FileNotFoundException("Arquivo bruto não encontrado.", path);
    }
}

public sealed record CargaBrutaSnapshotPreparado(
    bool DeveIngerir,
    string Fonte,
    string Dataset,
    string Escopo,
    string ChaveSnapshot,
    string HashSnapshot,
    string NomeArquivoOriginal,
    string CaminhoArquivoOriginal,
    long TamanhoBytes);
