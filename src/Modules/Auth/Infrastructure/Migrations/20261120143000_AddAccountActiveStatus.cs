using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ginecologia.Modules.Auth.Infrastructure.Migrations;

[DbContext(typeof(AuthDbContext))]
[Migration("20261120143000_AddAccountActiveStatus")]
public partial class AddAccountActiveStatus : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "is_active",
            schema: "auth",
            table: "accounts",
            type: "boolean",
            nullable: false,
            defaultValue: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropColumn(name: "is_active", schema: "auth", table: "accounts");
}
