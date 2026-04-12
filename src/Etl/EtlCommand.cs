namespace Etl;

public enum EtlSourceKind
{
    Tse,
    Ibge,
    Pnad
}

public sealed record EtlCommand(
    EtlSourceKind Kind,
    string Source,
    string FilePath,
    string? Dataset = null,
    int? Year = null);
