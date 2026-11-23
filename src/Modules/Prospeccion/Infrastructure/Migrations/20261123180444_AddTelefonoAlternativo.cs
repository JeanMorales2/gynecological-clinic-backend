using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ginecologia.Modules.Prospeccion.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTelefonoAlternativo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "telefono_alternativo",
                schema: "prospeccion",
                table: "prospects",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "telefono_alternativo",
                schema: "prospeccion",
                table: "prospects");
        }
    }
}
