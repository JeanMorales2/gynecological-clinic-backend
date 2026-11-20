using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ginecologia.Modules.Usuarios.Infrastructure.Migrations;

[DbContext(typeof(UsuariosDbContext))]
[Migration("20261120143001_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "usuarios");
        migrationBuilder.CreateTable(
            name: "perfiles",
            schema: "usuarios",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                nombre_completo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                telefono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                activo = table.Column<bool>(type: "boolean", nullable: false),
                created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            },
            constraints: table => table.PrimaryKey("PK_perfiles", x => x.id));
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(name: "perfiles", schema: "usuarios");
}
