using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace Ginecologia.Modules.Promociones.Infrastructure;
public sealed class PromocionesDesignTimeDbContextFactory : IDesignTimeDbContextFactory<PromocionesDbContext>
{ public PromocionesDbContext CreateDbContext(string[] args) { var connection = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") ?? "Host=localhost;Database=ginecologia;Username=postgres;Password=postgres"; return new(new DbContextOptionsBuilder<PromocionesDbContext>().UseNpgsql(connection).Options); } }
