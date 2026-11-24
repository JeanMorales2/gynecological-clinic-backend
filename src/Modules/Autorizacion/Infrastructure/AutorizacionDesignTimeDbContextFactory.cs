using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ginecologia.Modules.Autorizacion.Infrastructure;

public sealed class AutorizacionDesignTimeDbContextFactory : IDesignTimeDbContextFactory<AutorizacionDbContext>
{
    public AutorizacionDbContext CreateDbContext(string[] args) => new(
        new DbContextOptionsBuilder<AutorizacionDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=ginecologia;Username=ginecologia;Password=local-development-only", npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "autorizacion"))
            .Options);
}
