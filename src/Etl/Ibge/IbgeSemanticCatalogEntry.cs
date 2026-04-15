namespace Etl.Ibge;

public sealed record IbgeSemanticCatalogEntry(
    string FonteDicionario,
    string Pacote,
    string? Tipo,
    string? Tema,
    string Variavel,
    string? Categoria,
    string Descricao);
