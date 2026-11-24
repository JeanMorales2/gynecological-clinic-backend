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
        string? vicios,
        string? tipoSangre,
        int? gestas,
        int? partos,
        int? cesareas,
        int? abortos)
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
        TipoSangre = ValidBloodType(tipoSangre);
        Gestas = ValidObstetricCount(gestas, nameof(gestas));
        Partos = ValidObstetricCount(partos, nameof(partos));
        Cesareas = ValidObstetricCount(cesareas, nameof(cesareas));
        Abortos = ValidObstetricCount(abortos, nameof(abortos));
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
    public string? TipoSangre { get; private set; }
    public int? Gestas { get; private set; }
    public int? Partos { get; private set; }
    public int? Cesareas { get; private set; }
    public int? Abortos { get; private set; }
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
        string? vicios,
        string? tipoSangre = null,
        int? gestas = null,
        int? partos = null,
        int? cesareas = null,
        int? abortos = null) =>
        new(Guid.NewGuid(), nombreCompleto, fechaNacimiento, telefono, email, documentoIdentidad, alergias, antecedentesRelevantes, enfermedadesCronicas, etsIts, vicios, tipoSangre, gestas, partos, cesareas, abortos);

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
        string? vicios,
        string? tipoSangre = null,
        int? gestas = null,
        int? partos = null,
        int? cesareas = null,
        int? abortos = null)
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
        TipoSangre = ValidBloodType(tipoSangre);
        Gestas = ValidObstetricCount(gestas, nameof(gestas));
        Partos = ValidObstetricCount(partos, nameof(partos));
        Cesareas = ValidObstetricCount(cesareas, nameof(cesareas));
        Abortos = ValidObstetricCount(abortos, nameof(abortos));
    }

    public void Deactivate() => Activo = false;

    public void Activate() => Activo = true;

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

    private static readonly string[] ValidBloodTypes = { "O+", "O-", "A+", "A-", "B+", "B-", "AB+", "AB-" };

    private static string? ValidBloodType(string? value)
    {
        var normalized = NormalizeOptional(value);
        if (normalized is null) return null;
        if (!ValidBloodTypes.Contains(normalized)) throw new ArgumentException("El tipo de sangre no es válido.", nameof(value));
        return normalized;
    }

    private static int? ValidObstetricCount(int? value, string paramName)
    {
        if (value is null) return null;
        if (value < 0) throw new ArgumentException("El valor de la fórmula obstétrica no puede ser negativo.", paramName);
        return value;
    }
}
