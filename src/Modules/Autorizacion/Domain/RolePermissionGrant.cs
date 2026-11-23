namespace Ginecologia.Modules.Autorizacion.Domain;

public sealed class RolePermissionGrant
{
    private RolePermissionGrant() { }

    private RolePermissionGrant(string role, string permissionKey)
    {
        Role = role;
        PermissionKey = permissionKey;
    }

    public string Role { get; private set; } = string.Empty;
    public string PermissionKey { get; private set; } = string.Empty;

    public static RolePermissionGrant Create(string role, string permissionKey) => new(role, permissionKey);
}
