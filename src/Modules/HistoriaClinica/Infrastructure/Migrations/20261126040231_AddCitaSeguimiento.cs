using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ginecologia.Modules.HistoriaClinica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCitaSeguimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "cita_seguimiento",
                schema: "historia_clinica",
                table: "entries",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cita_seguimiento",
                schema: "historia_clinica",
                table: "entries");
        }
    }
}
