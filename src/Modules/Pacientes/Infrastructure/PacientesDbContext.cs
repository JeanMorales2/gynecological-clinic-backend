using Ginecologia.Modules.Pacientes.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.Pacientes.Infrastructure;

public sealed class PacientesDbContext(DbContextOptions<PacientesDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("pacientes");
        var patient = modelBuilder.Entity<Patient>();
        patient.ToTable("patients");
        patient.HasKey(x => x.Id);
        patient.Property(x => x.Id).HasColumnName("id");
        patient.Property(x => x.NombreCompleto).HasColumnName("nombre_completo").HasMaxLength(200).IsRequired();
        patient.Property(x => x.FechaNacimiento).HasColumnName("fecha_nacimiento").HasColumnType("date").IsRequired();
        patient.Property(x => x.Telefono).HasColumnName("telefono").HasMaxLength(50).IsRequired();
        patient.Property(x => x.Email).HasColumnName("email").HasMaxLength(320);
        patient.Property(x => x.DocumentoIdentidad).HasColumnName("documento_identidad").HasMaxLength(100);
        patient.Property(x => x.Alergias).HasColumnName("alergias").HasMaxLength(4000);
        patient.Property(x => x.AntecedentesRelevantes).HasColumnName("antecedentes_relevantes").HasMaxLength(8000);
        patient.Property(x => x.Activo).HasColumnName("activo").IsRequired();
        patient.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        patient.HasIndex(x => x.NombreCompleto).HasDatabaseName("ix_patients_nombre_completo");
    }
}
