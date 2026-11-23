namespace Ginecologia.Modules.Prospeccion.Domain;

public enum ProspectStatus
{
    Nuevo,
    Contactado,
    Agendado,
    Convertido,
    Perdido,
}

public sealed class Prospect
{
    private Prospect() { }

    private Prospect(Guid id, string nombreCompleto, string telefono, string? telefonoAlternativo, string? email, string? fuente, string? notas)
    {
        Id = id;
        NombreCompleto = RequiredName(nombreCompleto);
        Telefono = RequiredPhone(telefono);
        TelefonoAlternativo = NormalizeOptional(telefonoAlternativo);
        Email = NormalizeOptional(email);
        Fuente = NormalizeOptional(fuente);
        Notas = NormalizeOptional(notas);
        Estado = ProspectStatus.Nuevo;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string NombreCompleto { get; private set; } = string.Empty;
    public string Telefono { get; private set; } = string.Empty;
    public string? TelefonoAlternativo { get; private set; }
    public string? Email { get; private set; }
    public string? Fuente { get; private set; }
    public ProspectStatus Estado { get; private set; }
    public string? Notas { get; private set; }
    public Guid? PatientId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static Prospect Create(string nombreCompleto, string telefono, string? telefonoAlternativo, string? email, string? fuente, string? notas) =>
        new(Guid.NewGuid(), nombreCompleto, telefono, telefonoAlternativo, email, fuente, notas);

    public void Update(string nombreCompleto, string telefono, string? telefonoAlternativo, string? email, string? fuente, ProspectStatus estado, string? notas)
    {
        if (Estado == ProspectStatus.Convertido)
            throw new InvalidOperationException("Un prospecto convertido no se puede modificar desde el pipeline.");
        if (estado == ProspectStatus.Convertido)
            throw new ArgumentException("El estado Convertido solo se asigna al convertir el prospecto en paciente.", nameof(estado));

        NombreCompleto = RequiredName(nombreCompleto);
        Telefono = RequiredPhone(telefono);
        TelefonoAlternativo = NormalizeOptional(telefonoAlternativo);
        Email = NormalizeOptional(email);
        Fuente = NormalizeOptional(fuente);
        Estado = estado;
        Notas = NormalizeOptional(notas);
    }

    public void MarkAsConverted(Guid patientId)
    {
        if (patientId == Guid.Empty) throw new ArgumentException("El identificador del paciente es obligatorio.", nameof(patientId));
        if (Estado == ProspectStatus.Convertido) throw new InvalidOperationException("El prospecto ya fue convertido en paciente.");

        Estado = ProspectStatus.Convertido;
        PatientId = patientId;
    }

    public void MarcarContactado()
    {
        if (Estado == ProspectStatus.Nuevo) Estado = ProspectStatus.Contactado;
    }

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

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
