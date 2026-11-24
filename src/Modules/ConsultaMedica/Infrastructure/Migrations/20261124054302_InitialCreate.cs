using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ginecologia.Modules.ConsultaMedica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "consulta_medica");

            migrationBuilder.CreateTable(
                name: "consultas",
                schema: "consulta_medica",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    registrado_por_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    finalizada_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    presion_sistolica = table.Column<int>(type: "integer", nullable: true),
                    presion_diastolica = table.Column<int>(type: "integer", nullable: true),
                    peso_kg = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    talla_cm = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    fum = table.Column<DateOnly>(type: "date", nullable: true),
                    motivo_consulta = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    padecimiento_actual = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    hallazgos_mamas = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    hallazgos_cervix_genitales = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    hallazgos_tacto_bimanual = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ultrasonido_endometrio_mm = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    ultrasonido_ovario_derecho = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ultrasonido_ovario_izquierdo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ultrasonido_saco_gestacional_embrion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    diagnostico_cie = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    diagnostico_descripcion = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    estudios_solicitados = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    indicaciones_proxima_cita = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consultas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "consulta_attachments",
                schema: "consulta_medica",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    consulta_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre_archivo = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    tipo_contenido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    contenido = table.Column<byte[]>(type: "bytea", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consulta_attachments", x => x.id);
                    table.ForeignKey(
                        name: "FK_consulta_attachments_consultas_consulta_id",
                        column: x => x.consulta_id,
                        principalSchema: "consulta_medica",
                        principalTable: "consultas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prescription_items",
                schema: "consulta_medica",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    consulta_id = table.Column<Guid>(type: "uuid", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    medicamento = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    dosis = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    via = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    frecuencia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    duracion_texto = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prescription_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_prescription_items_consultas_consulta_id",
                        column: x => x.consulta_id,
                        principalSchema: "consulta_medica",
                        principalTable: "consultas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_consulta_attachments_consulta_id",
                schema: "consulta_medica",
                table: "consulta_attachments",
                column: "consulta_id");

            migrationBuilder.CreateIndex(
                name: "ix_consultas_patient_fecha",
                schema: "consulta_medica",
                table: "consultas",
                columns: new[] { "patient_id", "fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_prescription_items_consulta_id",
                schema: "consulta_medica",
                table: "prescription_items",
                column: "consulta_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "consulta_attachments",
                schema: "consulta_medica");

            migrationBuilder.DropTable(
                name: "prescription_items",
                schema: "consulta_medica");

            migrationBuilder.DropTable(
                name: "consultas",
                schema: "consulta_medica");
        }
    }
}
