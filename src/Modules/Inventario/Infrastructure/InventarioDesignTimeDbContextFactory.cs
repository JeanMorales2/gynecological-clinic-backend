using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace Ginecologia.Modules.Inventario.Infrastructure;
public sealed class InventarioDesignTimeDbContextFactory : IDesignTimeDbContextFactory<InventarioDbContext>
{ public InventarioDbContext CreateDbContext(string[] args) { var connection = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") ?? "Host=localhost;Database=ginecologia;Username=postgres;Password=postgres"; return new(new DbContextOptionsBuilder<InventarioDbContext>().UseNpgsql(connection).Options); } }
