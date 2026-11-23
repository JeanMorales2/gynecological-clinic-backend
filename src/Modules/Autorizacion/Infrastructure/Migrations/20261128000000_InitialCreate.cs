using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ginecologia.Modules.Autorizacion.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "autorizacion");

            migrationBuilder.CreateTable(
                name: "permission_definitions",
                schema: "autorizacion",
                columns: table => new
                {
                    key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission_definitions", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "role_permission_grants",
                schema: "autorizacion",
                columns: table => new
                {
                    role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    permission_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permission_grants", x => new { x.role, x.permission_key });
                });

            migrationBuilder.InsertData(
                schema: "autorizacion",
                table: "permission_definitions",
                columns: new[] { "key", "descripcion" },
                values: new object[,]
                {
                    { "usuarios.gestionar", "Gestionar usuarios y cuentas del personal" },
                    { "notificaciones.gestionar", "Gestionar notificaciones y probar el envío de correos" },
                    { "citas.gestionar_excepciones", "Configurar días de excepción en el horario de citas" },
                    { "historia_clinica.gestionar", "Crear y editar entradas de historia clínica" },
                    { "laboratorio.gestionar", "Gestionar órdenes y adjuntos de laboratorio externo" },
                    { "facturacion.gestionar", "Cobrar y anular facturas" },
                    { "inventario.gestionar", "Gestionar ítems e inventario de stock" },
                    { "promociones.gestionar", "Crear, descartar y eliminar promociones" }
                });

            migrationBuilder.InsertData(
                schema: "autorizacion",
                table: "role_permission_grants",
                columns: new[] { "role", "permission_key" },
                values: new object[,]
                {
                    { "Ginecologa", "historia_clinica.gestionar" },
                    { "Ginecologa", "laboratorio.gestionar" },
                    { "Recepcionista", "facturacion.gestionar" },
                    { "Recepcionista", "inventario.gestionar" },
                    { "Recepcionista", "promociones.gestionar" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "permission_definitions",
                schema: "autorizacion");

            migrationBuilder.DropTable(
                name: "role_permission_grants",
                schema: "autorizacion");
        }
    }
}
