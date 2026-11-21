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

    private ClinicalEntry(Guid id, Guid patientId, ClinicalEntryType tipo, DateOnly fecha, string motivo, string hallazgos, string? diagnostico, string? indicaciones, Guid registradoPorId)
    {
        Id = id;
        PatientId = RequiredId(patientId, nameof(patientId));
        Tipo = tipo;
        Fecha = fecha;
        Motivo = RequiredText(motivo, nameof(motivo), 300);
        Hallazgos = RequiredText(hallazgos, nameof(hallazgos), 8000);
        Diagnostico = OptionalText(diagnostico, 4000);
        Indicaciones = OptionalText(indicaciones, 4000);
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
    public Guid RegistradoPorId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static ClinicalEntry Create(Guid patientId, ClinicalEntryType tipo, DateOnly fecha, string motivo, string hallazgos, string? diagnostico, string? indicaciones, Guid registradoPorId) =>
        new(Guid.NewGuid(), patientId, tipo, fecha, motivo, hallazgos, diagnostico, indicaciones, registradoPorId);

    // Clinical records are never deleted. Updates are limited to correcting data-entry errors.
    public void Update(ClinicalEntryType tipo, DateOnly fecha, string motivo, string hallazgos, string? diagnostico, string? indicaciones)
    {
        Tipo = tipo;
        Fecha = fecha;
        Motivo = RequiredText(motivo, nameof(motivo), 300);
        Hallazgos = RequiredText(hallazgos, nameof(hallazgos), 8000);
        Diagnostico = OptionalText(diagnostico, 4000);
        Indicaciones = OptionalText(indicaciones, 4000);
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
