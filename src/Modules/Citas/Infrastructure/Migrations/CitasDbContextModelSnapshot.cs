using System;
using Ginecologia.Modules.Citas.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ginecologia.Modules.Citas.Infrastructure.Migrations;

[DbContext(typeof(CitasDbContext))]
public partial class CitasDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasDefaultSchema("citas").HasAnnotation("ProductVersion", "9.0.9").HasAnnotation("Relational:MaxIdentifierLength", 63);
        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);
        modelBuilder.Entity("Ginecologia.Modules.Citas.Domain.Appointment", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid").HasColumnName("id");
            b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
            b.Property<int>("DuracionMinutos").HasColumnType("integer").HasColumnName("duracion_minutos");
            b.Property<string>("Estado").IsRequired().HasMaxLength(30).HasColumnType("character varying(30)").HasColumnName("estado");
            b.Property<DateTime>("FechaHoraInicio").HasColumnType("timestamp with time zone").HasColumnName("fecha_hora_inicio");
            b.Property<string>("Notas").HasMaxLength(4000).HasColumnType("character varying(4000)").HasColumnName("notas");
            b.Property<Guid>("PatientId").HasColumnType("uuid").HasColumnName("patient_id");
            b.Property<Guid>("ProfesionalId").HasColumnType("uuid").HasColumnName("profesional_id");
            b.HasKey("Id");
            b.HasIndex("PatientId").HasDatabaseName("ix_appointments_patient");
            b.HasIndex("ProfesionalId", "FechaHoraInicio").HasDatabaseName("ix_appointments_profesional_inicio");
            b.ToTable("appointments", "citas");
        });
#pragma warning restore 612, 618
    }
}
