using Data;
using Microsoft.EntityFrameworkCore;

namespace Etl.Ibge;

public sealed class IbgeSemanticCatalogPersistenceService(PublicDataDbContext context)
{
    public async Task<int> PersistAsync(IbgeSemanticCatalog catalog, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        var variables = catalog.Entries
            .GroupBy(entry => new { entry.Pacote, entry.Variavel })
            .Select(group => group.First())
            .Select(entry => new IbgeCatalogoVariavelRecord
            {
                FonteDicionario = entry.FonteDicionario,
                Pacote = entry.Pacote,
                Tipo = entry.Tipo ?? string.Empty,
                Tema = entry.Tema ?? string.Empty,
                Variavel = entry.Variavel,
                Descricao = entry.Descricao
            })
            .ToList();

        var categorias = catalog.Entries
            .Where(entry => !string.IsNullOrWhiteSpace(entry.Categoria))
            .Select(entry => new IbgeCatalogoCategoriaRecord
            {
                FonteDicionario = entry.FonteDicionario,
                Pacote = entry.Pacote,
                Variavel = entry.Variavel,
                Categoria = entry.Categoria!,
                Descricao = entry.Descricao
            })
            .ToList();

        await using var transaction = await context.Database.BeginTransactionAsync(ct);
        await context.IbgeCatalogoCategorias.ExecuteDeleteAsync(ct);
        await context.IbgeCatalogoVariaveis.ExecuteDeleteAsync(ct);
        context.ChangeTracker.Clear();
        await context.IbgeCatalogoVariaveis.AddRangeAsync(variables, ct);
        await context.IbgeCatalogoCategorias.AddRangeAsync(categorias, ct);
        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return variables.Count;
    }
}
