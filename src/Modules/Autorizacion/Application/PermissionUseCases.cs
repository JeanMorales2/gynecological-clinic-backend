namespace Ginecologia.Modules.Autorizacion.Application;

public sealed class ListPermissionsUseCase(IRolePermissionRepository repository)
{
    public async Task<IReadOnlyList<PermissionDto>> ExecuteAsync(CancellationToken ct = default)
    {
        var definitions = await repository.ListDefinitionsAsync(ct);
        var grants = await repository.ListGrantsAsync(ct);
        return definitions
            .Select(definition => new PermissionDto(
                definition.Key,
                definition.Descripcion,
                grants.Where(grant => grant.PermissionKey == definition.Key).Select(grant => grant.Role).ToList()))
            .ToList();
    }
}

public sealed class SetPermissionGrantUseCase(IRolePermissionRepository repository)
{
    public async Task ExecuteAsync(string role, string permissionKey, bool otorgado, CancellationToken ct = default)
    {
        if (role == "Admin")
            throw new ArgumentException("Admin siempre tiene todos los permisos, no se puede modificar.");
        if (role is not ("Ginecologa" or "Recepcionista"))
            throw new ArgumentException("El rol indicado no es válido.");

        var definitions = await repository.ListDefinitionsAsync(ct);
        if (!definitions.Any(definition => definition.Key == permissionKey))
            throw new ArgumentException("La clave de permiso indicada no existe en el catálogo.");

        if (otorgado)
            await repository.GrantAsync(role, permissionKey, ct);
        else
            await repository.RevokeAsync(role, permissionKey, ct);
    }
}

public sealed class IsRolePermittedUseCase(IRolePermissionRepository repository)
{
    public Task<bool> ExecuteAsync(string role, string permissionKey, CancellationToken ct = default) =>
        role == "Admin" ? Task.FromResult(true) : repository.HasPermissionAsync(role, permissionKey, ct);
}
