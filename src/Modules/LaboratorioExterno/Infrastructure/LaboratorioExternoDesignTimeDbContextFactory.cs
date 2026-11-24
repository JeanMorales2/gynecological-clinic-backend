using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace Ginecologia.Modules.LaboratorioExterno.Infrastructure;
public sealed class LaboratorioExternoDesignTimeDbContextFactory : IDesignTimeDbContextFactory<LaboratorioExternoDbContext>
{ public LaboratorioExternoDbContext CreateDbContext(string[] args) => new(new DbContextOptionsBuilder<LaboratorioExternoDbContext>().UseNpgsql("Host=localhost;Port=5432;Database=ginecologia;Username=ginecologia;Password=local-development-only", npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "laboratorio_externo")).Options); }
