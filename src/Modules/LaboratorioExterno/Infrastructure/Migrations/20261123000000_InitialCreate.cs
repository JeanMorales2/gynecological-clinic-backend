using System;
using Ginecologia.Modules.LaboratorioExterno.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace Ginecologia.Modules.LaboratorioExterno.Infrastructure.Migrations;
[DbContext(typeof(LaboratorioExternoDbContext))]
[Migration("20261123000000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "laboratorio_externo");
        migrationBuilder.CreateTable(name: "lab_orders", schema: "laboratorio_externo", columns: table => new { id = table.Column<Guid>(type: "uuid", nullable: false), patient_id = table.Column<Guid>(type: "uuid", nullable: false), tipo_estudio = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false), laboratorio_nombre = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false), fecha_solicitud = table.Column<DateOnly>(type: "date", nullable: false), estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false), resultado_resumen = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true), fecha_resultado = table.Column<DateOnly>(type: "date", nullable: true), observaciones = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true), solicitado_por_id = table.Column<Guid>(type: "uuid", nullable: false) }, constraints: table => table.PrimaryKey("PK_lab_orders", x => x.id));
        migrationBuilder.CreateIndex(name: "ix_lab_orders_patient_fecha", schema: "laboratorio_externo", table: "lab_orders", columns: new[] { "patient_id", "fecha_solicitud" });
    }
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "lab_orders", schema: "laboratorio_externo");
}
