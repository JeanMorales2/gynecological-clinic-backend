using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ginecologia.Modules.Usuarios.Infrastructure;

public sealed class UsuariosDesignTimeDbContextFactory : IDesignTimeDbContextFactory<UsuariosDbContext>
{
    public UsuariosDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<UsuariosDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=ginecologia;Username=ginecologia;Password=local-development-only", npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "usuarios"))
            .Options;
        return new UsuariosDbContext(options);
    }
}
