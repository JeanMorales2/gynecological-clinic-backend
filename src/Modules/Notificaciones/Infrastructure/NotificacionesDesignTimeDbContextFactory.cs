using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ginecologia.Modules.Notificaciones.Infrastructure;

public sealed class NotificacionesDesignTimeDbContextFactory : IDesignTimeDbContextFactory<NotificacionesDbContext>
{
    public NotificacionesDbContext CreateDbContext(string[] args)
    {
        var connection = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") ?? "Host=localhost;Database=ginecologia;Username=postgres;Password=postgres";
        return new(new DbContextOptionsBuilder<NotificacionesDbContext>().UseNpgsql(connection, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "notificaciones")).Options);
    }
}
