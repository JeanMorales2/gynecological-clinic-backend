using System;
using Ginecologia.Modules.Inventario.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace Ginecologia.Modules.Inventario.Infrastructure.Migrations;
[DbContext(typeof(InventarioDbContext))]
[Migration("20261124000000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "inventario");
        migrationBuilder.CreateTable(name: "inventory_items", schema: "inventario", columns: table => new { id = table.Column<Guid>(nullable: false), nombre = table.Column<string>(maxLength: 150, nullable: false), categoria = table.Column<string>(maxLength: 80, nullable: false), unidad_medida = table.Column<string>(maxLength: 30, nullable: false), stock_actual = table.Column<int>(nullable: false), stock_minimo = table.Column<int>(nullable: false), precio_unitario = table.Column<decimal>(type: "numeric(18,2)", nullable: true), activo = table.Column<bool>(nullable: false), created_at_utc = table.Column<DateTime>(nullable: false), updated_at_utc = table.Column<DateTime>(nullable: false) }, constraints: table => table.PrimaryKey("PK_inventory_items", x => x.id));
        migrationBuilder.CreateTable(name: "stock_movements", schema: "inventario", columns: table => new { id = table.Column<Guid>(nullable: false), inventory_item_id = table.Column<Guid>(nullable: false), delta = table.Column<int>(nullable: false), motivo = table.Column<string>(maxLength: 200, nullable: false), stock_resultante = table.Column<int>(nullable: false), registrado_por_id = table.Column<Guid>(nullable: false), created_at_utc = table.Column<DateTime>(nullable: false) }, constraints: table => { table.PrimaryKey("PK_stock_movements", x => x.id); table.ForeignKey("FK_stock_movements_inventory_items_inventory_item_id", x => x.inventory_item_id, principalSchema: "inventario", principalTable: "inventory_items", principalColumn: "id", onDelete: ReferentialAction.Restrict); });
        migrationBuilder.CreateIndex(name: "IX_inventory_items_categoria", schema: "inventario", table: "inventory_items", column: "categoria"); migrationBuilder.CreateIndex(name: "IX_stock_movements_inventory_item_id_created_at_utc", schema: "inventario", table: "stock_movements", columns: new[] { "inventory_item_id", "created_at_utc" });
    }
    protected override void Down(MigrationBuilder migrationBuilder) { migrationBuilder.DropTable(name: "stock_movements", schema: "inventario"); migrationBuilder.DropTable(name: "inventory_items", schema: "inventario"); }
}
