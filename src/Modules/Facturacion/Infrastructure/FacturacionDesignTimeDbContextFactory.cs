using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace Ginecologia.Modules.Facturacion.Infrastructure;
public sealed class FacturacionDesignTimeDbContextFactory : IDesignTimeDbContextFactory<FacturacionDbContext>
{ public FacturacionDbContext CreateDbContext(string[] args) => new(new DbContextOptionsBuilder<FacturacionDbContext>().UseNpgsql("Host=localhost;Port=5432;Database=ginecologia;Username=ginecologia;Password=local-development-only").Options); }
