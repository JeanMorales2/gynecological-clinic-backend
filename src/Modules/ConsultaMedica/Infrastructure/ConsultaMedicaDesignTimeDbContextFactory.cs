using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace Ginecologia.Modules.ConsultaMedica.Infrastructure;
public sealed class ConsultaMedicaDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ConsultaMedicaDbContext> { public ConsultaMedicaDbContext CreateDbContext(string[] args) => new(new DbContextOptionsBuilder<ConsultaMedicaDbContext>().UseNpgsql("Host=localhost;Port=5432;Database=ginecologia;Username=ginecologia;Password=local-development-only", npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "consulta_medica")).Options); }
