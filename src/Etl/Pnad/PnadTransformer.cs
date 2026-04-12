using DadosTabulares.Domain.Pnad;
using DadosTabulares.Domain.Tse;

namespace Etl.Pnad;

public sealed class PnadTransformer
{
    public IReadOnlyList<DadoDesemprego> TransformarDesemprego(IEnumerable<PnadCsvRow> rows)
        => rows.Select(row => new DadoDesemprego(new UF(row.UfSigla), new Trimestre(row.Ano, row.Trimestre), row.Valor)).ToList();

    public IReadOnlyList<DadoInformalidade> TransformarInformalidade(IEnumerable<PnadCsvRow> rows)
        => rows.Select(row => new DadoInformalidade(new UF(row.UfSigla), new Trimestre(row.Ano, row.Trimestre), row.Valor)).ToList();

    public IReadOnlyList<DadoRendaMedia> TransformarRendaMedia(IEnumerable<PnadCsvRow> rows)
        => rows.Select(row => new DadoRendaMedia(new UF(row.UfSigla), new Trimestre(row.Ano, row.Trimestre), row.Valor)).ToList();
}
