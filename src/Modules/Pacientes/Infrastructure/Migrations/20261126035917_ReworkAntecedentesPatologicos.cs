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
            // Antes de tirar estas 3 columnas, se fusiona cualquier dato real que tengan dentro de
            // antecedentes_relevantes (que en este mismo Up() se renombra a antecedentes_patologicos_personales),
            // etiquetado por campo de origen. Nunca se pierde informacion clinica existente silenciosamente;
            // LEFT(...,8000) es solo una salvaguarda defensiva para no exceder el limite de la columna
            // en el caso extremo de que los 4 campos esten simultaneamente cerca de su longitud maxima.
            migrationBuilder.Sql("""
                UPDATE pacientes.patients
                SET antecedentes_relevantes = LEFT(
                    TRIM(BOTH E'\n' FROM
                        COALESCE(antecedentes_relevantes, '') ||
                        CASE WHEN enfermedades_cronicas IS NOT NULL AND enfermedades_cronicas <> '' THEN E'\nEnfermedades cronicas: ' || enfermedades_cronicas ELSE '' END ||
                        CASE WHEN ets_its IS NOT NULL AND ets_its <> '' THEN E'\nETS/ITS: ' || ets_its ELSE '' END ||
                        CASE WHEN vicios IS NOT NULL AND vicios <> '' THEN E'\nVicios: ' || vicios ELSE '' END
                    ), 8000)
                WHERE (enfermedades_cronicas IS NOT NULL AND enfermedades_cronicas <> '')
                   OR (ets_its IS NOT NULL AND ets_its <> '')
                   OR (vicios IS NOT NULL AND vicios <> '');
                """);

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
