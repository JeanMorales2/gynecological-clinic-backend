namespace Ginecologia.Modules.Pacientes.Domain;

public sealed class FamilyMedicalHistory
{
    private FamilyMedicalHistory() { }

    private FamilyMedicalHistory(Guid id, Guid patientId, string condicion, string parentesco)
    {
        Id = id;
        PatientId = patientId == Guid.Empty ? throw new ArgumentException("El paciente es obligatorio.", nameof(patientId)) : patientId;
        Condicion = RequiredText(condicion, nameof(condicion), 200);
        Parentesco = RequiredText(parentesco, nameof(parentesco), 100);
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public string Condicion { get; private set; } = string.Empty;
    public string Parentesco { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }

    public static FamilyMedicalHistory Create(Guid patientId, string condicion, string parentesco) =>
        new(Guid.NewGuid(), patientId, condicion, parentesco);

    private static string RequiredText(string? value, string parameterName, int maxLength)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized)) throw new ArgumentException("El campo es obligatorio.", parameterName);
        if (normalized.Length > maxLength) throw new ArgumentException($"El campo no puede superar {maxLength} caracteres.", parameterName);
        return normalized;
    }
}
