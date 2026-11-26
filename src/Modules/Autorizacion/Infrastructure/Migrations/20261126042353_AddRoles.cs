using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ginecologia.Modules.Autorizacion.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roles",
                schema: "autorizacion",
                columns: table => new
                {
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    es_protegido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.nombre);
                });

            migrationBuilder.InsertData(
                schema: "autorizacion",
                table: "roles",
                columns: new[] { "nombre", "es_protegido" },
                values: new object[,]
                {
                    { "Admin", true },
                    { "Recepcionista", true },
                    { "Ginecologa", true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "roles",
                schema: "autorizacion");
        }
    }
}
