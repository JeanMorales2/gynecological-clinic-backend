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

public sealed class ListGrantedPermissionKeysUseCase(IRolePermissionRepository repository)
{
    public async Task<IReadOnlyList<string>> ExecuteAsync(string role, CancellationToken ct = default)
    {
        var definitions = await repository.ListDefinitionsAsync(ct);
        if (role == "Admin") return definitions.Select(d => d.Key).ToList();

        var grants = await repository.ListGrantsAsync(ct);
        return grants.Where(g => g.Role == role).Select(g => g.PermissionKey).ToList();
    }
}

public sealed class SetPermissionGrantUseCase(IRolePermissionRepository repository, IRolRepository roles)
{
    public async Task ExecuteAsync(string role, string permissionKey, bool otorgado, CancellationToken ct = default)
    {
        if (role == "Admin")
            throw new ArgumentException("Admin siempre tiene todos los permisos, no se puede modificar.");
        if (await roles.FindByNombreAsync(role, ct) is null)
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
