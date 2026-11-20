using Ginecologia.Modules.Usuarios.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.Usuarios.Infrastructure;

public sealed class UsuariosDbContext(DbContextOptions<UsuariosDbContext> options) : DbContext(options)
{
    public DbSet<StaffProfile> Perfiles => Set<StaffProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("usuarios");
        var profile = modelBuilder.Entity<StaffProfile>();
        profile.ToTable("perfiles");
        profile.HasKey(x => x.Id);
        profile.Property(x => x.Id).HasColumnName("id");
        profile.Property(x => x.NombreCompleto).HasColumnName("nombre_completo").HasMaxLength(200).IsRequired();
        profile.Property(x => x.Telefono).HasColumnName("telefono").HasMaxLength(50);
        profile.Property(x => x.Role).HasColumnName("role").HasMaxLength(32).IsRequired();
        profile.Property(x => x.Activo).HasColumnName("activo").IsRequired();
        profile.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
    }
}
