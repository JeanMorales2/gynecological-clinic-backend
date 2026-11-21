using System;
using Ginecologia.Modules.HistoriaClinica.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ginecologia.Modules.HistoriaClinica.Infrastructure.Migrations;

[DbContext(typeof(HistoriaClinicaDbContext))]
[Migration("20261122000000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "historia_clinica");
        migrationBuilder.CreateTable(
            name: "entries",
            schema: "historia_clinica",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                fecha = table.Column<DateOnly>(type: "date", nullable: false),
                motivo = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                hallazgos = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: false),
                diagnostico = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                indicaciones = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                registrado_por_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            },
            constraints: table => table.PrimaryKey("PK_entries", x => x.id));
        migrationBuilder.CreateIndex(name: "ix_entries_patient_fecha", schema: "historia_clinica", table: "entries", columns: new[] { "patient_id", "fecha" });
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "entries", schema: "historia_clinica");
}
