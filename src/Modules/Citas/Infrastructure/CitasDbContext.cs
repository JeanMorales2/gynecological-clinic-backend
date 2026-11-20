using Ginecologia.Modules.Citas.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.Citas.Infrastructure;

public sealed class CitasDbContext(DbContextOptions<CitasDbContext> options) : DbContext(options)
{
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("citas");
        var appointment = modelBuilder.Entity<Appointment>();
        appointment.ToTable("appointments");
        appointment.HasKey(x => x.Id);
        appointment.Property(x => x.Id).HasColumnName("id");
        appointment.Property(x => x.PatientId).HasColumnName("patient_id").IsRequired();
        appointment.Property(x => x.ProfesionalId).HasColumnName("profesional_id").IsRequired();
        appointment.Property(x => x.FechaHoraInicio).HasColumnName("fecha_hora_inicio").HasColumnType("timestamp with time zone").IsRequired();
        appointment.Property(x => x.DuracionMinutos).HasColumnName("duracion_minutos").IsRequired();
        appointment.Property(x => x.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(30).IsRequired();
        appointment.Property(x => x.Notas).HasColumnName("notas").HasMaxLength(4000);
        appointment.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamp with time zone").IsRequired();
        appointment.Ignore(x => x.FechaHoraFin);
        appointment.HasIndex(x => new { x.ProfesionalId, x.FechaHoraInicio }).HasDatabaseName("ix_appointments_profesional_inicio");
        appointment.HasIndex(x => x.PatientId).HasDatabaseName("ix_appointments_patient");
    }
}
