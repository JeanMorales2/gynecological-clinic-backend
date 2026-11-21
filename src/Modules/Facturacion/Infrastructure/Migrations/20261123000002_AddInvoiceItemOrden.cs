using Ginecologia.Modules.Facturacion.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace Ginecologia.Modules.Facturacion.Infrastructure.Migrations;
[DbContext(typeof(FacturacionDbContext))]
[Migration("20261123000002_AddInvoiceItemOrden")]
public partial class AddInvoiceItemOrden : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(name: "orden", schema: "facturacion", table: "invoice_items", type: "integer", nullable: false, defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "orden", schema: "facturacion", table: "invoice_items");
    }
}
