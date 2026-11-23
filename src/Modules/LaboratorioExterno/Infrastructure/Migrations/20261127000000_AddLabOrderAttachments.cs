using System;
using Ginecologia.Modules.LaboratorioExterno.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ginecologia.Modules.LaboratorioExterno.Infrastructure.Migrations;

[DbContext(typeof(LaboratorioExternoDbContext))]
[Migration("20261127000000_AddLabOrderAttachments")]
public partial class AddLabOrderAttachments : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "lab_order_attachments",
            schema: "laboratorio_externo",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                lab_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                nombre_archivo = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                contenido = table.Column<byte[]>(type: "bytea", nullable: false),
                tamano_bytes = table.Column<int>(type: "integer", nullable: false),
                subido_por_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_lab_order_attachments", x => x.id);
                table.ForeignKey("FK_lab_order_attachments_lab_orders_lab_order_id", x => x.lab_order_id, principalSchema: "laboratorio_externo", principalTable: "lab_orders", principalColumn: "id", onDelete: ReferentialAction.Cascade);
            });
        migrationBuilder.CreateIndex(name: "ix_lab_order_attachments_lab_order_id", schema: "laboratorio_externo", table: "lab_order_attachments", column: "lab_order_id");
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "lab_order_attachments", schema: "laboratorio_externo");
}
