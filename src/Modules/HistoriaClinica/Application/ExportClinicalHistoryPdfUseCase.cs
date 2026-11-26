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

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(style => style.FontSize(10));
                page.Header().Row(row =>
                {
                    if (logo is not null) row.ConstantItem(75).Height(45).Image(logo);
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text("Dra. Mar\u00eda Isabela Pav\u00f3n \u2014 Ginec\u00f3loga y Obstetra").FontSize(14).SemiBold();
                        column.Item().Text("Historia cl\u00ednica").FontSize(11).FontColor(Colors.Grey.Darken1);
                    });
                });
                page.Content().PaddingVertical(15).Column(column =>
                {
                    column.Spacing(8);
                    column.Item().Text("Datos del paciente").FontSize(13).SemiBold();
                    column.Item().Text(text =>
                    {
                        text.Span("Nombre: ").SemiBold(); text.Span(patient.NombreCompleto);
                        text.Span("\nFecha de nacimiento: ").SemiBold(); text.Span(patient.FechaNacimiento.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                        text.Span("\nTel\u00e9fono: ").SemiBold(); text.Span(patient.Telefono);
                        if (!string.IsNullOrWhiteSpace(patient.DocumentoIdentidad)) { text.Span("\nDocumento de identidad: ").SemiBold(); text.Span(patient.DocumentoIdentidad); }
                        if (!string.IsNullOrWhiteSpace(patient.Alergias)) { text.Span("\nAlergias: ").SemiBold(); text.Span(patient.Alergias); }
                        if (!string.IsNullOrWhiteSpace(patient.AntecedentesPatologicosPersonales)) { text.Span("\nAntecedentes patológicos personales: ").SemiBold(); text.Span(patient.AntecedentesPatologicosPersonales); }
                        if (!string.IsNullOrWhiteSpace(patient.AntecedentesPatologicosFamiliares)) { text.Span("\nAntecedentes patológicos familiares: ").SemiBold(); text.Span(patient.AntecedentesPatologicosFamiliares); }
                        if (!string.IsNullOrWhiteSpace(patient.AntecedentesQuirurgicos)) { text.Span("\nAntecedentes quirúrgicos: ").SemiBold(); text.Span(patient.AntecedentesQuirurgicos); }
                        if (!string.IsNullOrWhiteSpace(patient.Menarca)) { text.Span("\nMenarca: ").SemiBold(); text.Span(patient.Menarca); }
                        if (!string.IsNullOrWhiteSpace(patient.Ciclos)) { text.Span("\nCiclos: ").SemiBold(); text.Span(patient.Ciclos); }
                        if (!string.IsNullOrWhiteSpace(patient.Ivsa)) { text.Span("\nIVSA: ").SemiBold(); text.Span(patient.Ivsa); }
                        if (patient.Fum is not null) { text.Span("\nFUM: ").SemiBold(); text.Span(patient.Fum.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)); }
                        if (!string.IsNullOrWhiteSpace(patient.Mpf)) { text.Span("\nMPF: ").SemiBold(); text.Span(patient.Mpf); }
                        if (!string.IsNullOrWhiteSpace(patient.Pap)) { text.Span("\nPAP: ").SemiBold(); text.Span(patient.Pap); }
                        if (!string.IsNullOrWhiteSpace(patient.Complicaciones)) { text.Span("\nComplicaciones: ").SemiBold(); text.Span(patient.Complicaciones); }
                    });
                    if (familyMedicalHistories.Count > 0)
                    {
                        column.Item().PaddingTop(5).Text("Antecedentes familiares").SemiBold();
                        foreach (var familyMedicalHistory in familyMedicalHistories)
                            column.Item().Text($"{familyMedicalHistory.Condicion} ({familyMedicalHistory.Parentesco})");
                    }
                    column.Item().PaddingTop(10).Text("Entradas cl\u00ednicas").FontSize(13).SemiBold();
                    if (clinicalEntries.Count == 0)
                        column.Item().Text("No hay entradas cl\u00ednicas registradas.").Italic().FontColor(Colors.Grey.Darken1);
                    foreach (var entry in clinicalEntries)
                    {
                        column.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(8).Column(entryColumn =>
                        {
                            entryColumn.Spacing(3);
                            entryColumn.Item().Text($"{entry.Fecha:dd/MM/yyyy} \u2014 {EntryTypeName(entry.Tipo)}").SemiBold();
                            EntryLine(entryColumn, "Motivo", entry.Motivo);
                            EntryLine(entryColumn, "Hallazgos", entry.Hallazgos);
                            if (!string.IsNullOrWhiteSpace(entry.Tratamiento)) EntryLine(entryColumn, "Tratamiento", entry.Tratamiento);
                            if (!string.IsNullOrWhiteSpace(entry.Medicamentos)) EntryLine(entryColumn, "Medicamentos", entry.Medicamentos);
                            if (!string.IsNullOrWhiteSpace(entry.Examenes)) EntryLine(entryColumn, "Exámenes", entry.Examenes);
                            if (entry.PesoKg is not null || entry.AlturaCm is not null)
                            {
                                var measurements = new List<string>();
                                if (entry.PesoKg is not null) measurements.Add($"Peso: {entry.PesoKg.Value:F2} kg");
                                if (entry.AlturaCm is not null) measurements.Add($"Altura: {entry.AlturaCm.Value:F1} cm");
                                entryColumn.Item().Text(string.Join(" · ", measurements));
                            }
                            if (!string.IsNullOrWhiteSpace(entry.Diagnostico)) EntryLine(entryColumn, "Diagn\u00f3stico", entry.Diagnostico);
                            if (!string.IsNullOrWhiteSpace(entry.Indicaciones)) EntryLine(entryColumn, "Indicaciones", entry.Indicaciones);
                            if (entry.CitaSeguimiento is not null) EntryLine(entryColumn, "Cita de seguimiento", entry.CitaSeguimiento.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
                        });
                    }
                });
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Generado: ").FontSize(8); text.Span(generatedAt.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)).FontSize(8);
                    text.Span(" (Nicaragua) \u2022 P\u00e1gina ").FontSize(8); text.CurrentPageNumber().FontSize(8); text.Span(" de ").FontSize(8); text.TotalPages().FontSize(8);
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

    private static void EntryLine(ColumnDescriptor column, string label, string value) =>
        column.Item().Text(text => { text.Span($"{label}: ").SemiBold(); text.Span(value); });

    private static string EntryTypeName(ClinicalEntryType type) => type switch
    {
        ClinicalEntryType.Consulta => "Consulta",
        ClinicalEntryType.ControlPrenatal => "Control prenatal",
        ClinicalEntryType.Citologia => "Citolog\u00eda",
        ClinicalEntryType.Ecografia => "Ecograf\u00eda",
        ClinicalEntryType.Laboratorio => "Laboratorio",
        _ => type.ToString()
    };
}
