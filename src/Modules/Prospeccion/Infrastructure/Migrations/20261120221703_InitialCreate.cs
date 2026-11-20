using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

// Generated for the Prospeccion module's initial schema.

namespace Ginecologia.Modules.Prospeccion.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "prospeccion");

            migrationBuilder.CreateTable(
                name: "prospects",
                schema: "prospeccion",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre_completo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    telefono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    fuente = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    notas = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prospects", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_prospects_estado",
                schema: "prospeccion",
                table: "prospects",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "ix_prospects_nombre_completo",
                schema: "prospeccion",
                table: "prospects",
                column: "nombre_completo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "prospects",
                schema: "prospeccion");
        }
    }
}
