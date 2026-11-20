using Ginecologia.Modules.Prospeccion.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.Prospeccion.Infrastructure;

public sealed class ProspeccionDbContext(DbContextOptions<ProspeccionDbContext> options) : DbContext(options)
{
    public DbSet<Prospect> Prospects => Set<Prospect>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("prospeccion");
        var prospect = modelBuilder.Entity<Prospect>();
        prospect.ToTable("prospects");
        prospect.HasKey(x => x.Id);
        prospect.Property(x => x.Id).HasColumnName("id");
        prospect.Property(x => x.NombreCompleto).HasColumnName("nombre_completo").HasMaxLength(200).IsRequired();
        prospect.Property(x => x.Telefono).HasColumnName("telefono").HasMaxLength(50).IsRequired();
        prospect.Property(x => x.Email).HasColumnName("email").HasMaxLength(320);
        prospect.Property(x => x.Fuente).HasColumnName("fuente").HasMaxLength(200);
        prospect.Property(x => x.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20).IsRequired();
        prospect.Property(x => x.Notas).HasColumnName("notas").HasMaxLength(8000);
        prospect.Property(x => x.PatientId).HasColumnName("patient_id");
        prospect.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        prospect.HasIndex(x => x.NombreCompleto).HasDatabaseName("ix_prospects_nombre_completo");
        prospect.HasIndex(x => x.Estado).HasDatabaseName("ix_prospects_estado");
    }
}
