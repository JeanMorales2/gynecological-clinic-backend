namespace Ginecologia.Modules.HistoriaClinica.Domain;

public enum ClinicalEntryType
{
    Consulta,
    ControlPrenatal,
    Citologia,
    Ecografia,
    Laboratorio,
}

public sealed class ClinicalEntry
{
    private ClinicalEntry() { }

    private ClinicalEntry(Guid id, Guid patientId, ClinicalEntryType tipo, DateOnly fecha, string motivo, string hallazgos, string? diagnostico, string? indicaciones, string? tratamiento, string? medicamentos, string? examenes, decimal? pesoKg, decimal? alturaCm, Guid registradoPorId)
    {
        Id = id;
        PatientId = RequiredId(patientId, nameof(patientId));
        Tipo = tipo;
        Fecha = fecha;
        Motivo = RequiredText(motivo, nameof(motivo), 300);
        Hallazgos = RequiredText(hallazgos, nameof(hallazgos), 8000);
        Diagnostico = OptionalText(diagnostico, 4000);
        Indicaciones = OptionalText(indicaciones, 4000);
        Tratamiento = OptionalText(tratamiento, 4000);
        Medicamentos = OptionalText(medicamentos, 4000);
        Examenes = OptionalText(examenes, 4000);
        PesoKg = ValidMeasurement(pesoKg, nameof(pesoKg));
        AlturaCm = ValidMeasurement(alturaCm, nameof(alturaCm));
        RegistradoPorId = RequiredId(registradoPorId, nameof(registradoPorId));
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public ClinicalEntryType Tipo { get; private set; }
    public DateOnly Fecha { get; private set; }
    public string Motivo { get; private set; } = string.Empty;
    public string Hallazgos { get; private set; } = string.Empty;
    public string? Diagnostico { get; private set; }
    public string? Indicaciones { get; private set; }
    public string? Tratamiento { get; private set; }
    public string? Medicamentos { get; private set; }
    public string? Examenes { get; private set; }
    public decimal? PesoKg { get; private set; }
    public decimal? AlturaCm { get; private set; }
    public Guid RegistradoPorId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static ClinicalEntry Create(Guid patientId, ClinicalEntryType tipo, DateOnly fecha, string motivo, string hallazgos, string? diagnostico, string? indicaciones, string? tratamiento, string? medicamentos, string? examenes, decimal? pesoKg, decimal? alturaCm, Guid registradoPorId) =>
        new(Guid.NewGuid(), patientId, tipo, fecha, motivo, hallazgos, diagnostico, indicaciones, tratamiento, medicamentos, examenes, pesoKg, alturaCm, registradoPorId);

    // Clinical records are never deleted. Updates are limited to correcting data-entry errors.
    public void Update(ClinicalEntryType tipo, DateOnly fecha, string motivo, string hallazgos, string? diagnostico, string? indicaciones, string? tratamiento, string? medicamentos, string? examenes, decimal? pesoKg, decimal? alturaCm)
    {
        Tipo = tipo;
        Fecha = fecha;
        Motivo = RequiredText(motivo, nameof(motivo), 300);
        Hallazgos = RequiredText(hallazgos, nameof(hallazgos), 8000);
        Diagnostico = OptionalText(diagnostico, 4000);
        Indicaciones = OptionalText(indicaciones, 4000);
        Tratamiento = OptionalText(tratamiento, 4000);
        Medicamentos = OptionalText(medicamentos, 4000);
        Examenes = OptionalText(examenes, 4000);
        PesoKg = ValidMeasurement(pesoKg, nameof(pesoKg));
        AlturaCm = ValidMeasurement(alturaCm, nameof(alturaCm));
    }

    private static decimal? ValidMeasurement(decimal? value, string parameterName)
    {
        if (value is null) return null;
        if (value <= 0) throw new ArgumentException("El valor debe ser mayor que cero.", parameterName);
        return value;
    }

    private static Guid RequiredId(Guid value, string parameterName)
    {
        if (value == Guid.Empty) throw new ArgumentException("El identificador es obligatorio.", parameterName);
        return value;
    }

    private static string RequiredText(string? value, string parameterName, int maxLength)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized)) throw new ArgumentException("El campo es obligatorio.", parameterName);
        if (normalized.Length > maxLength) throw new ArgumentException($"El campo no puede superar {maxLength} caracteres.", parameterName);
        return normalized;
    }

    private static string? OptionalText(string? value, int maxLength)
    {
        var normalized = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        if (normalized is not null && normalized.Length > maxLength) throw new ArgumentException($"El campo no puede superar {maxLength} caracteres.", nameof(value));
        return normalized;
    }
}
