using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ginecologia.Modules.Pacientes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "pacientes");

            migrationBuilder.CreateTable(
                name: "patients",
                schema: "pacientes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre_completo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    fecha_nacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    telefono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    documento_identidad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    alergias = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    antecedentes_relevantes = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patients", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_patients_nombre_completo",
                schema: "pacientes",
                table: "patients",
                column: "nombre_completo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "patients",
                schema: "pacientes");
        }
    }
}
