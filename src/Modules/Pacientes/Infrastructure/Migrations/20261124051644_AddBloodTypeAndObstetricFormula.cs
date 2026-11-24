using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ginecologia.Modules.Pacientes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBloodTypeAndObstetricFormula : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "abortos",
                schema: "pacientes",
                table: "patients",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "cesareas",
                schema: "pacientes",
                table: "patients",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "gestas",
                schema: "pacientes",
                table: "patients",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "partos",
                schema: "pacientes",
                table: "patients",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tipo_sangre",
                schema: "pacientes",
                table: "patients",
                type: "character varying(5)",
                maxLength: 5,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "abortos",
                schema: "pacientes",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "cesareas",
                schema: "pacientes",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "gestas",
                schema: "pacientes",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "partos",
                schema: "pacientes",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "tipo_sangre",
                schema: "pacientes",
                table: "patients");
        }
    }
}
