namespace Ginecologia.Modules.Usuarios.Domain;

public sealed class StaffProfile
{
    private StaffProfile() { }

    private StaffProfile(Guid id, string nombreCompleto, string? telefono, string role, DateTime createdAtUtc)
    {
        Id = id;
        NombreCompleto = nombreCompleto;
        Telefono = telefono;
        Role = role;
        Activo = true;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; private set; }
    public string NombreCompleto { get; private set; } = string.Empty;
    public string? Telefono { get; private set; }
    public string Role { get; private set; } = string.Empty;
    public bool Activo { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static StaffProfile Create(Guid id, string nombreCompleto, string? telefono, string role)
    {
        if (id == Guid.Empty) throw new ArgumentException("El identificador es obligatorio.", nameof(id));
        return new StaffProfile(id, RequiredName(nombreCompleto), NormalizePhone(telefono), role, DateTime.UtcNow);
    }

    public void Update(string nombreCompleto, string? telefono, string role)
    {
        NombreCompleto = RequiredName(nombreCompleto);
        Telefono = NormalizePhone(telefono);
        Role = role;
    }

    public void Deactivate() => Activo = false;

    public void Reactivate() => Activo = true;

    private static string RequiredName(string value)
    {
        var name = value.Trim();
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("El nombre completo es obligatorio.", nameof(value));
        return name;
    }

    private static string? NormalizePhone(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
