namespace Ginecologia.Modules.Pacientes.Domain;

public sealed class Patient
{
    private Patient() { }

    private Patient(
        Guid id,
        string nombreCompleto,
        DateOnly fechaNacimiento,
        string telefono,
        string? email,
        string? documentoIdentidad,
        string? alergias,
        string? antecedentesRelevantes,
        string? enfermedadesCronicas,
        string? etsIts,
        string? vicios)
    {
        Id = id;
        NombreCompleto = RequiredName(nombreCompleto);
        FechaNacimiento = ValidBirthDate(fechaNacimiento);
        Telefono = RequiredPhone(telefono);
        Email = NormalizeOptional(email);
        DocumentoIdentidad = NormalizeOptional(documentoIdentidad);
        Alergias = NormalizeOptional(alergias);
        AntecedentesRelevantes = NormalizeOptional(antecedentesRelevantes);
        EnfermedadesCronicas = NormalizeOptional(enfermedadesCronicas);
        EtsIts = NormalizeOptional(etsIts);
        Vicios = NormalizeOptional(vicios);
        Activo = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string NombreCompleto { get; private set; } = string.Empty;
    public DateOnly FechaNacimiento { get; private set; }
    public string Telefono { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? DocumentoIdentidad { get; private set; }
    public string? Alergias { get; private set; }
    public string? AntecedentesRelevantes { get; private set; }
    public string? EnfermedadesCronicas { get; private set; }
    public string? EtsIts { get; private set; }
    public string? Vicios { get; private set; }
    public bool Activo { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static Patient Create(
        string nombreCompleto,
        DateOnly fechaNacimiento,
        string telefono,
        string? email,
        string? documentoIdentidad,
        string? alergias,
        string? antecedentesRelevantes,
        string? enfermedadesCronicas,
        string? etsIts,
        string? vicios) =>
        new(Guid.NewGuid(), nombreCompleto, fechaNacimiento, telefono, email, documentoIdentidad, alergias, antecedentesRelevantes, enfermedadesCronicas, etsIts, vicios);

    public void Update(
        string nombreCompleto,
        DateOnly fechaNacimiento,
        string telefono,
        string? email,
        string? documentoIdentidad,
        string? alergias,
        string? antecedentesRelevantes,
        string? enfermedadesCronicas,
        string? etsIts,
        string? vicios)
    {
        NombreCompleto = RequiredName(nombreCompleto);
        FechaNacimiento = ValidBirthDate(fechaNacimiento);
        Telefono = RequiredPhone(telefono);
        Email = NormalizeOptional(email);
        DocumentoIdentidad = NormalizeOptional(documentoIdentidad);
        Alergias = NormalizeOptional(alergias);
        AntecedentesRelevantes = NormalizeOptional(antecedentesRelevantes);
        EnfermedadesCronicas = NormalizeOptional(enfermedadesCronicas);
        EtsIts = NormalizeOptional(etsIts);
        Vicios = NormalizeOptional(vicios);
    }

    public void Deactivate() => Activo = false;

    private static string RequiredName(string value)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized)) throw new ArgumentException("El nombre completo es obligatorio.", nameof(value));
        return normalized;
    }

    private static string RequiredPhone(string value)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized)) throw new ArgumentException("El teléfono es obligatorio.", nameof(value));
        return normalized;
    }

    private static DateOnly ValidBirthDate(DateOnly value)
    {
        if (value > DateOnly.FromDateTime(DateTime.UtcNow)) throw new ArgumentException("La fecha de nacimiento no puede ser futura.", nameof(value));
        return value;
    }

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
