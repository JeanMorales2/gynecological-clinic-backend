using System;
using Ginecologia.Modules.Citas.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ginecologia.Modules.Citas.Infrastructure.Migrations;

[DbContext(typeof(CitasDbContext))]
[Migration("20261126000000_AddScheduleExceptions")]
public partial class AddScheduleExceptions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "schedule_exceptions",
            schema: "citas",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                fecha = table.Column<DateOnly>(type: "date", nullable: false),
                descripcion = table.Column<string>(type: "text", nullable: false),
                created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            },
            constraints: table => table.PrimaryKey("PK_schedule_exceptions", x => x.id));

        migrationBuilder.CreateIndex(
            name: "ix_schedule_exceptions_fecha",
            schema: "citas",
            table: "schedule_exceptions",
            column: "fecha",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(name: "schedule_exceptions", schema: "citas");
}
