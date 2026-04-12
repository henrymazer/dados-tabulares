using DadosTabulares.Domain.Tse;

namespace DadosTabulares.Etl.Tse;

public sealed class TseIngestionService(TseCsvParser parser, TseIngestionLoader loader)
{
    public Task<int> IngerirAsync(TseIngestionSource source, TextReader csv, AnoEleicao anoEleicao, CancellationToken ct = default)
        => source switch
        {
            TseIngestionSource.Resultados => IngerirResultadosAsync(csv, anoEleicao, ct),
            TseIngestionSource.Eleitorado => IngerirEleitoradoAsync(csv, anoEleicao, ct),
            TseIngestionSource.Bens => IngerirBensAsync(csv, anoEleicao, ct),
            TseIngestionSource.PrestacaoContas => IngerirPrestacaoContasAsync(csv, anoEleicao, ct),
            TseIngestionSource.Coligacoes => IngerirColigacoesAsync(csv, anoEleicao, ct),
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, "Fonte TSE desconhecida.")
        };

    public async Task<int> IngerirResultadosAsync(TextReader csv, AnoEleicao anoEleicao, CancellationToken ct = default)
        => await loader.IngerirResultadosAsync(await parser.ParseResultadosAsync(csv, anoEleicao, ct: ct), ct);

    public async Task<int> IngerirEleitoradoAsync(TextReader csv, AnoEleicao anoEleicao, CancellationToken ct = default)
        => await loader.IngerirEleitoradoAsync(await parser.ParseEleitoradoAsync(csv, anoEleicao, ct), ct);

    public async Task<int> IngerirBensAsync(TextReader csv, AnoEleicao anoEleicao, CancellationToken ct = default)
        => await loader.IngerirBensAsync(await parser.ParseBensDeclaradosAsync(csv, anoEleicao, ct), ct);

    public async Task<int> IngerirPrestacaoContasAsync(TextReader csv, AnoEleicao anoEleicao, CancellationToken ct = default)
        => await loader.IngerirPrestacoesContasAsync(await parser.ParsePrestacaoContasAsync(csv, anoEleicao, ct), ct);

    public async Task<int> IngerirColigacoesAsync(TextReader csv, AnoEleicao anoEleicao, CancellationToken ct = default)
        => await loader.IngerirColigacoesAsync(await parser.ParseColigacoesAsync(csv, anoEleicao, ct), ct);
}
