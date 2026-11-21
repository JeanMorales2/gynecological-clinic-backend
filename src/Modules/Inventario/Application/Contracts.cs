using Ginecologia.Modules.Inventario.Domain;
namespace Ginecologia.Modules.Inventario.Application;
public interface IInventoryItemRepository { Task<InventoryItem?> FindByIdAsync(Guid id, CancellationToken ct = default); Task<PagedInventoryResult<InventoryItem>> ListAsync(InventoryListQuery query, CancellationToken ct = default); Task AddAsync(InventoryItem item, CancellationToken ct = default); Task SaveChangesAsync(CancellationToken ct = default); }
public interface IStockMovementRepository { Task<PagedInventoryResult<StockMovement>> ListByItemAsync(Guid itemId, int page, int pageSize, CancellationToken ct = default); Task AddAsync(StockMovement movement, CancellationToken ct = default); }
public sealed record InventoryListQuery(string? Search, string? Categoria, bool SoloBajoStock, int Page, int PageSize);
public sealed record PagedInventoryResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
public sealed record InventoryItemDto(Guid Id, string Nombre, string Categoria, string UnidadMedida, int StockActual, int StockMinimo, decimal? PrecioUnitario, bool Activo, DateTime CreatedAtUtc, DateTime UpdatedAtUtc) { public static InventoryItemDto From(InventoryItem x) => new(x.Id, x.Nombre, x.Categoria, x.UnidadMedida, x.StockActual, x.StockMinimo, x.PrecioUnitario, x.Activo, x.CreatedAtUtc, x.UpdatedAtUtc); }
public sealed record StockMovementDto(Guid Id, Guid InventoryItemId, int Delta, string Motivo, int StockResultante, Guid RegistradoPorId, DateTime CreatedAtUtc) { public static StockMovementDto From(StockMovement x) => new(x.Id, x.InventoryItemId, x.Delta, x.Motivo, x.StockResultante, x.RegistradoPorId, x.CreatedAtUtc); }
public sealed class InventoryItemNotFoundException(Guid id) : KeyNotFoundException($"No existe el ítem de inventario {id}.");
