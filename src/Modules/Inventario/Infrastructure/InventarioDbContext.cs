using Ginecologia.Modules.Inventario.Domain;
using Microsoft.EntityFrameworkCore;
namespace Ginecologia.Modules.Inventario.Infrastructure;
public sealed class InventarioDbContext(DbContextOptions<InventarioDbContext> options) : DbContext(options)
{
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>(); public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("inventario");
        var item = modelBuilder.Entity<InventoryItem>(); item.ToTable("inventory_items"); item.HasKey(x => x.Id); item.Property(x => x.Id).HasColumnName("id"); item.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(150).IsRequired(); item.Property(x => x.Categoria).HasColumnName("categoria").HasMaxLength(80).IsRequired(); item.Property(x => x.UnidadMedida).HasColumnName("unidad_medida").HasMaxLength(30).IsRequired(); item.Property(x => x.StockActual).HasColumnName("stock_actual").IsRequired(); item.Property(x => x.StockMinimo).HasColumnName("stock_minimo").IsRequired(); item.Property(x => x.PrecioUnitario).HasColumnName("precio_unitario").HasPrecision(18, 2); item.Property(x => x.Activo).HasColumnName("activo").IsRequired(); item.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired(); item.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").IsRequired(); item.HasIndex(x => x.Categoria);
        var movement = modelBuilder.Entity<StockMovement>(); movement.ToTable("stock_movements"); movement.HasKey(x => x.Id); movement.Property(x => x.Id).HasColumnName("id"); movement.Property(x => x.InventoryItemId).HasColumnName("inventory_item_id").IsRequired(); movement.Property(x => x.Delta).HasColumnName("delta").IsRequired(); movement.Property(x => x.Motivo).HasColumnName("motivo").HasMaxLength(200).IsRequired(); movement.Property(x => x.StockResultante).HasColumnName("stock_resultante").IsRequired(); movement.Property(x => x.RegistradoPorId).HasColumnName("registrado_por_id").IsRequired(); movement.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired(); movement.HasIndex(x => new { x.InventoryItemId, x.CreatedAtUtc }); movement.HasOne<InventoryItem>().WithMany().HasForeignKey(x => x.InventoryItemId).OnDelete(DeleteBehavior.Restrict);
    }
}
