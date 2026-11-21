using Ginecologia.Modules.Notificaciones.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.Notificaciones.Infrastructure;

public sealed class NotificacionesDbContext(DbContextOptions<NotificacionesDbContext> options) : DbContext(options)
{
    public DbSet<EmailNotification> EmailNotifications => Set<EmailNotification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("notificaciones");
        var notification = modelBuilder.Entity<EmailNotification>();
        notification.ToTable("email_notifications");
        notification.HasKey(x => x.Id);
        notification.Property(x => x.Id).HasColumnName("id");
        notification.Property(x => x.Destinatario).HasColumnName("destinatario").HasMaxLength(320).IsRequired();
        notification.Property(x => x.Asunto).HasColumnName("asunto").HasMaxLength(200).IsRequired();
        notification.Property(x => x.Cuerpo).HasColumnName("cuerpo").HasMaxLength(5000).IsRequired();
        notification.Property(x => x.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20).IsRequired();
        notification.Property(x => x.IntentosCount).HasColumnName("intentos_count").IsRequired();
        notification.Property(x => x.UltimoError).HasColumnName("ultimo_error").HasMaxLength(1000);
        notification.Property(x => x.EnviadoAtUtc).HasColumnName("enviado_at_utc");
        notification.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        notification.HasIndex(x => new { x.Estado, x.CreatedAtUtc });
    }
}
