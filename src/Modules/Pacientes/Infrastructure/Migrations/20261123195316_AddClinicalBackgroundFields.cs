using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ginecologia.Modules.Pacientes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClinicalBackgroundFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "enfermedades_cronicas",
                schema: "pacientes",
                table: "patients",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ets_its",
                schema: "pacientes",
                table: "patients",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "vicios",
                schema: "pacientes",
                table: "patients",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "family_medical_histories",
                schema: "pacientes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    condicion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    parentesco = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_family_medical_histories", x => x.id);
                    table.ForeignKey(
                        name: "FK_family_medical_histories_patients_patient_id",
                        column: x => x.patient_id,
                        principalSchema: "pacientes",
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_family_medical_histories_patient_id",
                schema: "pacientes",
                table: "family_medical_histories",
                column: "patient_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "family_medical_histories",
                schema: "pacientes");

            migrationBuilder.DropColumn(
                name: "enfermedades_cronicas",
                schema: "pacientes",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "ets_its",
                schema: "pacientes",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "vicios",
                schema: "pacientes",
                table: "patients");
        }
    }
}
