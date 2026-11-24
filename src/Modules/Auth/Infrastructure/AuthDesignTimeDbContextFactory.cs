using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ginecologia.Modules.Auth.Infrastructure;

public sealed class AuthDesignTimeDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=ginecologia;Username=ginecologia;Password=local-development-only", npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "auth"))
            .Options;
        return new AuthDbContext(options);
    }
}
