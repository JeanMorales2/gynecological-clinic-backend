using Ginecologia.Modules.Inventario.Application;
using Ginecologia.Modules.Inventario.Domain;
using Microsoft.EntityFrameworkCore;
namespace Ginecologia.Modules.Inventario.Infrastructure;
public sealed class InventoryItemRepository(InventarioDbContext db) : IInventoryItemRepository
{
    public Task<InventoryItem?> FindByIdAsync(Guid id, CancellationToken ct = default) => db.InventoryItems.SingleOrDefaultAsync(x => x.Id == id, ct);
    public async Task<PagedInventoryResult<InventoryItem>> ListAsync(InventoryListQuery query, CancellationToken ct = default) { IQueryable<InventoryItem> source = db.InventoryItems.AsNoTracking(); if (!string.IsNullOrWhiteSpace(query.Search)) source = source.Where(x => EF.Functions.ILike(x.Nombre, $"%{query.Search}%")); if (!string.IsNullOrWhiteSpace(query.Categoria)) source = source.Where(x => EF.Functions.ILike(x.Categoria, $"%{query.Categoria}%")); if (query.SoloBajoStock) source = source.Where(x => x.StockActual <= x.StockMinimo); var total = await source.CountAsync(ct); var items = await source.OrderBy(x => x.Nombre).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct); return new(items, total, query.Page, query.PageSize); }
    public Task AddAsync(InventoryItem item, CancellationToken ct = default) => db.InventoryItems.AddAsync(item, ct).AsTask(); public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
public sealed class StockMovementRepository(InventarioDbContext db) : IStockMovementRepository
{ public async Task<PagedInventoryResult<StockMovement>> ListByItemAsync(Guid id, int page, int pageSize, CancellationToken ct = default) { var source = db.StockMovements.AsNoTracking().Where(x => x.InventoryItemId == id); var total = await source.CountAsync(ct); var items = await source.OrderByDescending(x => x.CreatedAtUtc).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct); return new(items, total, page, pageSize); } public Task AddAsync(StockMovement item, CancellationToken ct = default) => db.StockMovements.AddAsync(item, ct).AsTask(); }
