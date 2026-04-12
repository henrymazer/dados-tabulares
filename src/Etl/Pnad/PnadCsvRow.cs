namespace Etl.Pnad;

public sealed record PnadCsvRow(string UfSigla, int Ano, int Trimestre, decimal Valor);
