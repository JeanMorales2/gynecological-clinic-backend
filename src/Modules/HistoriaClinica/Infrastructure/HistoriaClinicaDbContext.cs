using Ginecologia.Modules.HistoriaClinica.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.HistoriaClinica.Infrastructure;

public sealed class HistoriaClinicaDbContext(DbContextOptions<HistoriaClinicaDbContext> options) : DbContext(options)
{
    public DbSet<ClinicalEntry> ClinicalEntries => Set<ClinicalEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("historia_clinica");
        var entry = modelBuilder.Entity<ClinicalEntry>();
        entry.ToTable("entries");
        entry.HasKey(x => x.Id);
        entry.Property(x => x.Id).HasColumnName("id");
        entry.Property(x => x.PatientId).HasColumnName("patient_id").IsRequired();
        entry.Property(x => x.Tipo).HasColumnName("tipo").HasConversion<string>().HasMaxLength(30).IsRequired();
        entry.Property(x => x.Fecha).HasColumnName("fecha").HasColumnType("date").IsRequired();
        entry.Property(x => x.Motivo).HasColumnName("motivo").HasMaxLength(300).IsRequired();
        entry.Property(x => x.Hallazgos).HasColumnName("hallazgos").HasMaxLength(8000).IsRequired();
        entry.Property(x => x.Diagnostico).HasColumnName("diagnostico").HasMaxLength(4000);
        entry.Property(x => x.Indicaciones).HasColumnName("indicaciones").HasMaxLength(4000);
        entry.Property(x => x.RegistradoPorId).HasColumnName("registrado_por_id").IsRequired();
        entry.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamp with time zone").IsRequired();
        entry.HasIndex(x => new { x.PatientId, x.Fecha }).HasDatabaseName("ix_entries_patient_fecha");
    }
}
