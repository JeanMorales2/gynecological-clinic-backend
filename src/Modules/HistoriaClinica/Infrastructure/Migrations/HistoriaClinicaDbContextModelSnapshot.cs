using System;
using Ginecologia.Modules.HistoriaClinica.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ginecologia.Modules.HistoriaClinica.Infrastructure.Migrations;

[DbContext(typeof(HistoriaClinicaDbContext))]
public partial class HistoriaClinicaDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasDefaultSchema("historia_clinica").HasAnnotation("ProductVersion", "9.0.9").HasAnnotation("Relational:MaxIdentifierLength", 63);
        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);
        modelBuilder.Entity("Ginecologia.Modules.HistoriaClinica.Domain.ClinicalEntry", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uuid").HasColumnName("id");
            b.Property<DateTime>("CreatedAtUtc").HasColumnType("timestamp with time zone").HasColumnName("created_at_utc");
            b.Property<string>("Diagnostico").HasMaxLength(4000).HasColumnType("character varying(4000)").HasColumnName("diagnostico");
            b.Property<DateOnly>("Fecha").HasColumnType("date").HasColumnName("fecha");
            b.Property<string>("Hallazgos").IsRequired().HasMaxLength(8000).HasColumnType("character varying(8000)").HasColumnName("hallazgos");
            b.Property<string>("Indicaciones").HasMaxLength(4000).HasColumnType("character varying(4000)").HasColumnName("indicaciones");
            b.Property<string>("Motivo").IsRequired().HasMaxLength(300).HasColumnType("character varying(300)").HasColumnName("motivo");
            b.Property<Guid>("PatientId").HasColumnType("uuid").HasColumnName("patient_id");
            b.Property<Guid>("RegistradoPorId").HasColumnType("uuid").HasColumnName("registrado_por_id");
            b.Property<string>("Tipo").IsRequired().HasMaxLength(30).HasColumnType("character varying(30)").HasColumnName("tipo");
            b.HasKey("Id");
            b.HasIndex("PatientId", "Fecha").HasDatabaseName("ix_entries_patient_fecha");
            b.ToTable("entries", "historia_clinica");
        });
#pragma warning restore 612, 618
    }
}
