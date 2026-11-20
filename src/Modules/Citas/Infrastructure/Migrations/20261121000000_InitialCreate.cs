using System;
using Ginecologia.Modules.Citas.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ginecologia.Modules.Citas.Infrastructure.Migrations;

[DbContext(typeof(CitasDbContext))]
[Migration("20261121000000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "citas");
        migrationBuilder.CreateTable(
            name: "appointments",
            schema: "citas",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                profesional_id = table.Column<Guid>(type: "uuid", nullable: false),
                fecha_hora_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                duracion_minutos = table.Column<int>(type: "integer", nullable: false),
                estado = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                notas = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            },
            constraints: table => table.PrimaryKey("PK_appointments", x => x.id));
        migrationBuilder.CreateIndex(name: "ix_appointments_patient", schema: "citas", table: "appointments", column: "patient_id");
        migrationBuilder.CreateIndex(name: "ix_appointments_profesional_inicio", schema: "citas", table: "appointments", columns: new[] { "profesional_id", "fecha_hora_inicio" });
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "appointments", schema: "citas");
}
