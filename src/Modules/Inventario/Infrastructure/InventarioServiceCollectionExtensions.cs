using Ginecologia.Modules.Inventario.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace Ginecologia.Modules.Inventario.Infrastructure;
public static class InventarioServiceCollectionExtensions
{ public static IServiceCollection AddInventarioModule(this IServiceCollection services, string connectionString) { services.AddDbContext<InventarioDbContext>(x => x.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "inventario"))); services.AddScoped<IInventoryItemRepository, InventoryItemRepository>(); services.AddScoped<IStockMovementRepository, StockMovementRepository>(); services.AddScoped<CreateInventoryItemUseCase>(); services.AddScoped<UpdateInventoryItemUseCase>(); services.AddScoped<AdjustStockUseCase>(); services.AddScoped<DeactivateInventoryItemUseCase>(); services.AddScoped<ReactivateInventoryItemUseCase>(); services.AddScoped<GetInventoryItemUseCase>(); services.AddScoped<ListInventoryItemsUseCase>(); services.AddScoped<ListStockMovementsUseCase>(); return services; } }
