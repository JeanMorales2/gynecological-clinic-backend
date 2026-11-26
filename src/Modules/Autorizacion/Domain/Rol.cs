namespace Ginecologia.Modules.Autorizacion.Domain;

public sealed class Rol
{
    private Rol() { }

    private Rol(string nombre, bool esProtegido)
    {
        Nombre = RequiredName(nombre);
        EsProtegido = esProtegido;
    }

    public string Nombre { get; private set; } = string.Empty;
    public bool EsProtegido { get; private set; }

    public static Rol Create(string nombre) => new(nombre, false);

    public static Rol CreateProtegido(string nombre) => new(nombre, true);

    private static string RequiredName(string value)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized)) throw new ArgumentException("El nombre del rol es obligatorio.", nameof(value));
        if (normalized.Length > 50) throw new ArgumentException("El nombre del rol no puede superar 50 caracteres.", nameof(value));
        return normalized;
    }
}
