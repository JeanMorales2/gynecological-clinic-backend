using System;
using Ginecologia.Modules.Facturacion.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace Ginecologia.Modules.Facturacion.Infrastructure.Migrations;
[DbContext(typeof(FacturacionDbContext))]
[Migration("20261123000001_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "facturacion");
        migrationBuilder.CreateTable(name: "invoices", schema: "facturacion", columns: table => new { id = table.Column<Guid>(type: "uuid", nullable: false), patient_id = table.Column<Guid>(type: "uuid", nullable: false), appointment_id = table.Column<Guid>(type: "uuid", nullable: true), numero_factura = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false), fecha = table.Column<DateOnly>(type: "date", nullable: false), monto_total = table.Column<decimal>(type: "numeric(18,2)", nullable: false), estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false), metodo_pago = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true), fecha_pago = table.Column<DateOnly>(type: "date", nullable: true), creado_por_id = table.Column<Guid>(type: "uuid", nullable: false) }, constraints: table => table.PrimaryKey("PK_invoices", x => x.id));
        migrationBuilder.CreateTable(name: "invoice_items", schema: "facturacion", columns: table => new { id = table.Column<Guid>(type: "uuid", nullable: false), invoice_id = table.Column<Guid>(type: "uuid", nullable: false), concepto = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false), cantidad = table.Column<int>(type: "integer", nullable: false), precio_unitario = table.Column<decimal>(type: "numeric(18,2)", nullable: false) }, constraints: table => { table.PrimaryKey("PK_invoice_items", x => x.id); table.ForeignKey(name: "FK_invoice_items_invoices_invoice_id", column: x => x.invoice_id, principalSchema: "facturacion", principalTable: "invoices", principalColumn: "id", onDelete: ReferentialAction.Cascade); });
        migrationBuilder.CreateIndex(name: "IX_invoices_numero_factura", schema: "facturacion", table: "invoices", column: "numero_factura", unique: true);
        migrationBuilder.CreateIndex(name: "ix_invoices_patient_fecha", schema: "facturacion", table: "invoices", columns: new[] { "patient_id", "fecha" });
        migrationBuilder.CreateIndex(name: "IX_invoice_items_invoice_id", schema: "facturacion", table: "invoice_items", column: "invoice_id");
    }
    protected override void Down(MigrationBuilder migrationBuilder) { migrationBuilder.DropTable(name: "invoice_items", schema: "facturacion"); migrationBuilder.DropTable(name: "invoices", schema: "facturacion"); }
}
