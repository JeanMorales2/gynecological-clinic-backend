using System.Globalization;
using System.Text;
using Ginecologia.Modules.HistoriaClinica.Domain;
using Ginecologia.Modules.Pacientes.Application;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Ginecologia.Modules.HistoriaClinica.Application;

public sealed class ExportClinicalHistoryPdfUseCase(IClinicalEntryRepository entries, GetPatientUseCase getPatient, ListFamilyMedicalHistoryUseCase listFamilyMedicalHistory)
{
    private const string BrandColor = "#4B1F20";
    private const string BoxBorderColor = "#F1CFD3";
    private const string BoxBackgroundColor = "#FDF6F7";

    public async Task<byte[]> ExecuteAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        if (patientId == Guid.Empty) throw new ArgumentException("El paciente es obligatorio.", nameof(patientId));
        var patient = await getPatient.ExecuteAsync(patientId, cancellationToken) ?? throw new ArgumentException("El paciente indicado no existe.", nameof(patientId));
        var clinicalEntries = await entries.ListAllByPatientIdAsync(patientId, cancellationToken);
        var familyMedicalHistories = await listFamilyMedicalHistory.ExecuteAsync(patientId, cancellationToken);
        var logoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "clinic-logo.png");
        var logo = File.Exists(logoPath) ? File.ReadAllBytes(logoPath) : null;
        var nicaragua = TimeZoneInfo.FindSystemTimeZoneById("America/Managua");
        var generatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, nicaragua);
        var age = CalculateAge(patient.FechaNacimiento, DateOnly.FromDateTime(generatedAt));

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(style => style.FontSize(9.5f));
                page.Header().Column(header =>
                {
                    header.Item().Row(row =>
                    {
                        if (logo is not null) row.ConstantItem(70).Height(42).Image(logo);
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("Dra. María Isabela Pavón — Ginecóloga y Obstetra").FontSize(12).SemiBold();
                            column.Item().Text("HISTORIA CLÍNICA").FontSize(16).Bold().FontColor(BrandColor);
                        });
                    });
                    header.Item().PaddingTop(6).LineHorizontal(1.5f).LineColor(BrandColor);
                });
                page.Content().PaddingVertical(12).Column(column =>
                {
                    column.Spacing(10);

                    Banner(column, "Datos del paciente");
                    Box(column, box =>
                    {
                        FieldRow(box, ("Nombre", patient.NombreCompleto), ("Edad", $"{age} años"));
                        FieldRow(box,
                            ("Fecha de nacimiento", patient.FechaNacimiento.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)),
                            ("Teléfono", patient.Telefono));
                        FieldRow(box, ("Documento de identidad", patient.DocumentoIdentidad), ("Correo", patient.Email));
                        FieldRow(box, ("Tipo de sangre", patient.TipoSangre), ("Alergias", patient.Alergias));
                    });

                    column.Item().Text("1. Antecedentes").FontSize(12).Bold().FontColor(BrandColor);

                    column.Item().Text("a) Antecedentes patológicos familiares").SemiBold();
                    Box(column, box =>
                    {
                        box.Item().Text(string.IsNullOrWhiteSpace(patient.AntecedentesPatologicosFamiliares) ? "No registrado." : patient.AntecedentesPatologicosFamiliares);
                        if (familyMedicalHistories.Count > 0)
                            foreach (var familyMedicalHistory in familyMedicalHistories)
                                box.Item().Text($"• {familyMedicalHistory.Condicion} ({familyMedicalHistory.Parentesco})");
                    });

                    column.Item().Text("b) Antecedentes patológicos personales").SemiBold();
                    Box(column, box => box.Item().Text(string.IsNullOrWhiteSpace(patient.AntecedentesPatologicosPersonales) ? "No registrado." : patient.AntecedentesPatologicosPersonales));

                    column.Item().Text("c) Antecedentes quirúrgicos").SemiBold();
                    Box(column, box => box.Item().Text(string.IsNullOrWhiteSpace(patient.AntecedentesQuirurgicos) ? "No registrado." : patient.AntecedentesQuirurgicos));

                    column.Item().Text("d) Antecedentes gineco-obstétricos").SemiBold();
                    Box(column, box =>
                    {
                        var hadAny = false;
                        hadAny |= FieldRow(box, ("Menarca", patient.Menarca), ("Ciclos", patient.Ciclos), ("IVSA", patient.Ivsa));
                        hadAny |= FieldRow(box, ("FUM", patient.Fum?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)), ("MPF", patient.Mpf), ("PAP", patient.Pap));
                        hadAny |= FieldRow(box,
                            ("Gestas", patient.Gestas?.ToString(CultureInfo.InvariantCulture)),
                            ("Partos", patient.Partos?.ToString(CultureInfo.InvariantCulture)),
                            ("Cesáreas", patient.Cesareas?.ToString(CultureInfo.InvariantCulture)),
                            ("Abortos", patient.Abortos?.ToString(CultureInfo.InvariantCulture)));
                        hadAny |= FieldRow(box, ("Complicaciones", patient.Complicaciones));
                        if (!hadAny) box.Item().Text("No registrado.");
                    });

                    column.Item().PaddingTop(4).Text("2. Entradas clínicas").FontSize(12).Bold().FontColor(BrandColor);
                    if (clinicalEntries.Count == 0)
                        column.Item().Text("No hay entradas clínicas registradas.").Italic().FontColor(Colors.Grey.Darken1);
                    foreach (var entry in clinicalEntries)
                    {
                        Banner(column, $"{entry.Fecha:dd/MM/yyyy} — {EntryTypeName(entry.Tipo)}");
                        Box(column, box =>
                        {
                            FieldLine(box, "Motivo de consulta", entry.Motivo);
                            FieldLine(box, "Hallazgos", entry.Hallazgos);
                        });
                        if (entry.PesoKg is not null || entry.AlturaCm is not null)
                        {
                            column.Item().Text("Exploración física").SemiBold();
                            Box(column, box =>
                            {
                                var measurements = new List<(string, string?)>();
                                if (entry.PesoKg is not null) measurements.Add(("Peso", $"{entry.PesoKg.Value:F2} kg"));
                                if (entry.AlturaCm is not null) measurements.Add(("Talla", $"{entry.AlturaCm.Value:F1} cm"));
                                FieldRow(box, measurements.ToArray());
                            });
                        }
                        if (!string.IsNullOrWhiteSpace(entry.Diagnostico))
                        {
                            column.Item().Text("Diagnóstico").SemiBold();
                            Box(column, box => box.Item().Text(entry.Diagnostico));
                        }
                        if (!string.IsNullOrWhiteSpace(entry.Tratamiento) || !string.IsNullOrWhiteSpace(entry.Medicamentos) || !string.IsNullOrWhiteSpace(entry.Indicaciones) || !string.IsNullOrWhiteSpace(entry.Examenes) || entry.CitaSeguimiento is not null)
                        {
                            column.Item().Text("Plan terapéutico").SemiBold();
                            Box(column, box =>
                            {
                                FieldLine(box, "Tratamiento", entry.Tratamiento);
                                FieldLine(box, "Medicamentos", entry.Medicamentos);
                                FieldLine(box, "Indicaciones", entry.Indicaciones);
                                FieldLine(box, "Exámenes", entry.Examenes);
                                if (entry.CitaSeguimiento is not null) FieldLine(box, "Cita de seguimiento", entry.CitaSeguimiento.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                            });
                        }
                    }
                });
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Generado: ").FontSize(8); text.Span(generatedAt.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)).FontSize(8);
                    text.Span(" (Nicaragua) • Página ").FontSize(8); text.CurrentPageNumber().FontSize(8); text.Span(" de ").FontSize(8); text.TotalPages().FontSize(8);
                });
            });
        }).GeneratePdf();
    }

    public static string SuggestedFileName(string fullName)
    {
        var normalized = fullName.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();
        foreach (var character in normalized)
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark && char.IsLetterOrDigit(character)) builder.Append(character);
        return $"historia-clinica-{builder}.pdf";
    }

    private static int CalculateAge(DateOnly birthDate, DateOnly asOf)
    {
        var age = asOf.Year - birthDate.Year;
        if (asOf < birthDate.AddYears(age)) age--;
        return Math.Max(age, 0);
    }

    private static void Banner(ColumnDescriptor column, string title) =>
        column.Item().Background(BrandColor).Padding(6).Text(title).FontColor(Colors.White).Bold().FontSize(10.5f);

    private static void Box(ColumnDescriptor column, Action<ColumnDescriptor> content) =>
        column.Item().Border(1).BorderColor(BoxBorderColor).Background(BoxBackgroundColor).Padding(8).Column(box => { box.Spacing(2); content(box); });

    private static bool FieldRow(ColumnDescriptor column, params (string label, string? value)[] fields)
    {
        var present = fields.Where(field => !string.IsNullOrWhiteSpace(field.value)).ToArray();
        if (present.Length == 0) return false;
        column.Item().Row(row =>
        {
            foreach (var (label, value) in present)
                row.RelativeItem().Text(text => { text.Span($"{label}: ").SemiBold(); text.Span(value!); });
        });
        return true;
    }

    private static void FieldLine(ColumnDescriptor column, string label, string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        column.Item().Text(text => { text.Span($"{label}: ").SemiBold(); text.Span(value); });
    }

    private static string EntryTypeName(ClinicalEntryType type) => type switch
    {
        ClinicalEntryType.Consulta => "Consulta",
        ClinicalEntryType.ControlPrenatal => "Control prenatal",
        ClinicalEntryType.Citologia => "Citología",
        ClinicalEntryType.Ecografia => "Ecografía",
        ClinicalEntryType.Laboratorio => "Laboratorio",
        _ => type.ToString()
    };
}
