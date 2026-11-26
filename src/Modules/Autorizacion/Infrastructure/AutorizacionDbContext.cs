using Ginecologia.Modules.Autorizacion.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.Autorizacion.Infrastructure;

public sealed class AutorizacionDbContext(DbContextOptions<AutorizacionDbContext> options) : DbContext(options)
{
    public DbSet<PermissionDefinition> PermissionDefinitions => Set<PermissionDefinition>();
    public DbSet<RolePermissionGrant> RolePermissionGrants => Set<RolePermissionGrant>();
    public DbSet<Rol> Roles => Set<Rol>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("autorizacion");

        var rol = modelBuilder.Entity<Rol>();
        rol.ToTable("roles");
        rol.HasKey(x => x.Nombre);
        rol.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(50);
        rol.Property(x => x.EsProtegido).HasColumnName("es_protegido").IsRequired();

        var definition = modelBuilder.Entity<PermissionDefinition>();
        definition.ToTable("permission_definitions");
        definition.HasKey(x => x.Key);
        definition.Property(x => x.Key).HasColumnName("key").HasMaxLength(100);
        definition.Property(x => x.Descripcion).HasColumnName("descripcion").HasMaxLength(300).IsRequired();

        var grant = modelBuilder.Entity<RolePermissionGrant>();
        grant.ToTable("role_permission_grants");
        grant.HasKey(x => new { x.Role, x.PermissionKey });
        grant.Property(x => x.Role).HasColumnName("role").HasMaxLength(50);
        grant.Property(x => x.PermissionKey).HasColumnName("permission_key").HasMaxLength(100);
    }
}
