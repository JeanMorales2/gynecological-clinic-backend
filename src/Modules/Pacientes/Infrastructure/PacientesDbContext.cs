using Ginecologia.Modules.Pacientes.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.Pacientes.Infrastructure;

public sealed class PacientesDbContext(DbContextOptions<PacientesDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<FamilyMedicalHistory> FamilyMedicalHistories => Set<FamilyMedicalHistory>();

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
        patient.Property(x => x.EnfermedadesCronicas).HasColumnName("enfermedades_cronicas").HasMaxLength(4000);
        patient.Property(x => x.EtsIts).HasColumnName("ets_its").HasMaxLength(4000);
        patient.Property(x => x.Vicios).HasColumnName("vicios").HasMaxLength(2000);
        patient.Property(x => x.TipoSangre).HasColumnName("tipo_sangre").HasMaxLength(5);
        patient.Property(x => x.Gestas).HasColumnName("gestas");
        patient.Property(x => x.Partos).HasColumnName("partos");
        patient.Property(x => x.Cesareas).HasColumnName("cesareas");
        patient.Property(x => x.Abortos).HasColumnName("abortos");
        patient.Property(x => x.Activo).HasColumnName("activo").IsRequired();
        patient.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        patient.HasIndex(x => x.NombreCompleto).HasDatabaseName("ix_patients_nombre_completo");

        var familyHistory = modelBuilder.Entity<FamilyMedicalHistory>();
        familyHistory.ToTable("family_medical_histories");
        familyHistory.HasKey(x => x.Id);
        familyHistory.Property(x => x.Id).HasColumnName("id");
        familyHistory.Property(x => x.PatientId).HasColumnName("patient_id").IsRequired();
        familyHistory.Property(x => x.Condicion).HasColumnName("condicion").HasMaxLength(200).IsRequired();
        familyHistory.Property(x => x.Parentesco).HasColumnName("parentesco").HasMaxLength(100).IsRequired();
        familyHistory.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        familyHistory.HasOne<Patient>().WithMany().HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.Cascade);
        familyHistory.HasIndex(x => x.PatientId).HasDatabaseName("ix_family_medical_histories_patient_id");
    }
}
