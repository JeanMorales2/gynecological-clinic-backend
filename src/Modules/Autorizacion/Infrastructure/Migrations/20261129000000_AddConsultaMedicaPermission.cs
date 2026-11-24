using Ginecologia.Modules.Autorizacion.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ginecologia.Modules.Autorizacion.Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AutorizacionDbContext))]
    [Migration("20261129000000_AddConsultaMedicaPermission")]
    public partial class AddConsultaMedicaPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "INSERT INTO autorizacion.permission_definitions (key, descripcion) VALUES ('consultas.gestionar', 'Crear, editar y finalizar consultas médicas');");

            migrationBuilder.Sql(
                "INSERT INTO autorizacion.role_permission_grants (role, permission_key) VALUES ('Ginecologa', 'consultas.gestionar');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM autorizacion.role_permission_grants WHERE role = 'Ginecologa' AND permission_key = 'consultas.gestionar';");

            migrationBuilder.Sql(
                "DELETE FROM autorizacion.permission_definitions WHERE key = 'consultas.gestionar';");
        }
    }
}
