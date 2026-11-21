using System;
using Ginecologia.Modules.Notificaciones.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable

namespace Ginecologia.Modules.Notificaciones.Infrastructure.Migrations;

[DbContext(typeof(NotificacionesDbContext))]
[Migration("20261125000000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "notificaciones");
        migrationBuilder.CreateTable(
            name: "email_notifications",
            schema: "notificaciones",
            columns: table => new
            {
                id = table.Column<Guid>(nullable: false),
                destinatario = table.Column<string>(maxLength: 320, nullable: false),
                asunto = table.Column<string>(maxLength: 200, nullable: false),
                cuerpo = table.Column<string>(maxLength: 5000, nullable: false),
                estado = table.Column<string>(maxLength: 20, nullable: false),
                intentos_count = table.Column<int>(nullable: false),
                ultimo_error = table.Column<string>(maxLength: 1000, nullable: true),
                enviado_at_utc = table.Column<DateTime>(nullable: true),
                created_at_utc = table.Column<DateTime>(nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_email_notifications", x => x.id));
        migrationBuilder.CreateIndex(name: "IX_email_notifications_estado_created_at_utc", schema: "notificaciones", table: "email_notifications", columns: new[] { "estado", "created_at_utc" });
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "email_notifications", schema: "notificaciones");
}
