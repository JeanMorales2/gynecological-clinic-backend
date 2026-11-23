using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ginecologia.Modules.HistoriaClinica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExtendedClinicalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "altura_cm",
                schema: "historia_clinica",
                table: "entries",
                type: "numeric(5,1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "examenes",
                schema: "historia_clinica",
                table: "entries",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "medicamentos",
                schema: "historia_clinica",
                table: "entries",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "peso_kg",
                schema: "historia_clinica",
                table: "entries",
                type: "numeric(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tratamiento",
                schema: "historia_clinica",
                table: "entries",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "altura_cm",
                schema: "historia_clinica",
                table: "entries");

            migrationBuilder.DropColumn(
                name: "examenes",
                schema: "historia_clinica",
                table: "entries");

            migrationBuilder.DropColumn(
                name: "medicamentos",
                schema: "historia_clinica",
                table: "entries");

            migrationBuilder.DropColumn(
                name: "peso_kg",
                schema: "historia_clinica",
                table: "entries");

            migrationBuilder.DropColumn(
                name: "tratamiento",
                schema: "historia_clinica",
                table: "entries");
        }
    }
}
