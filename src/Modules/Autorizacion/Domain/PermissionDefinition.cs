namespace Ginecologia.Modules.Autorizacion.Domain;

public sealed class PermissionDefinition
{
    private PermissionDefinition() { }

    private PermissionDefinition(string key, string descripcion)
    {
        Key = key;
        Descripcion = descripcion;
    }

    public string Key { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;

    public static PermissionDefinition Create(string key, string descripcion) => new(key, descripcion);
}
