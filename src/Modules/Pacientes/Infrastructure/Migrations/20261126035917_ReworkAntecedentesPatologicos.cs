using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ginecologia.Modules.Pacientes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReworkAntecedentesPatologicos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "enfermedades_cronicas",
                schema: "pacientes",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "vicios",
                schema: "pacientes",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "ets_its",
                schema: "pacientes",
                table: "patients");

            migrationBuilder.RenameColumn(
                name: "antecedentes_relevantes",
                schema: "pacientes",
                table: "patients",
                newName: "antecedentes_patologicos_personales");

            migrationBuilder.AddColumn<string>(
                name: "antecedentes_patologicos_familiares",
                schema: "pacientes",
                table: "patients",
                type: "character varying(8000)",
                maxLength: 8000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "antecedentes_quirurgicos",
                schema: "pacientes",
                table: "patients",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "complicaciones",
                schema: "pacientes",
                table: "patients",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ciclos",
                schema: "pacientes",
                table: "patients",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "fum",
                schema: "pacientes",
                table: "patients",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ivsa",
                schema: "pacientes",
                table: "patients",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "menarca",
                schema: "pacientes",
                table: "patients",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "mpf",
                schema: "pacientes",
                table: "patients",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pap",
                schema: "pacientes",
                table: "patients",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "antecedentes_patologicos_familiares",
                schema: "pacientes",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "antecedentes_quirurgicos",
                schema: "pacientes",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "complicaciones",
                schema: "pacientes",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "ciclos",
                schema: "pacientes",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "fum",
                schema: "pacientes",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "ivsa",
                schema: "pacientes",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "menarca",
                schema: "pacientes",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "mpf",
                schema: "pacientes",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "pap",
                schema: "pacientes",
                table: "patients");

            migrationBuilder.RenameColumn(
                name: "antecedentes_patologicos_personales",
                schema: "pacientes",
                table: "patients",
                newName: "antecedentes_relevantes");

            migrationBuilder.AddColumn<string>(
                name: "enfermedades_cronicas",
                schema: "pacientes",
                table: "patients",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ets_its",
                schema: "pacientes",
                table: "patients",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "vicios",
                schema: "pacientes",
                table: "patients",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }
    }
}
