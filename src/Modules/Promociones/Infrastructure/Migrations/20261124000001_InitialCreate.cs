using System;
using Ginecologia.Modules.Promociones.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace Ginecologia.Modules.Promociones.Infrastructure.Migrations;
[DbContext(typeof(PromocionesDbContext))]
[Migration("20261124000001_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "promociones");
        migrationBuilder.CreateTable(name: "promotions", schema: "promociones", columns: table => new { id = table.Column<Guid>(nullable: false), patient_id = table.Column<Guid>(nullable: false), tipo = table.Column<string>(maxLength: 30, nullable: false), titulo = table.Column<string>(maxLength: 250, nullable: false), mensaje = table.Column<string>(maxLength: 2000, nullable: false), fecha_generada = table.Column<DateOnly>(type: "date", nullable: false), estado = table.Column<string>(maxLength: 30, nullable: false), created_at_utc = table.Column<DateTime>(nullable: false) }, constraints: table => table.PrimaryKey("PK_promotions", x => x.id));
        migrationBuilder.CreateIndex(name: "IX_promotions_patient_id_estado", schema: "promociones", table: "promotions", columns: new[] { "patient_id", "estado" }); migrationBuilder.CreateIndex(name: "IX_promotions_patient_id_tipo_fecha_generada", schema: "promociones", table: "promotions", columns: new[] { "patient_id", "tipo", "fecha_generada" }, unique: true);
    }
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "promotions", schema: "promociones");
}
