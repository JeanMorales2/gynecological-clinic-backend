using Ginecologia.Modules.Autorizacion.Domain;

namespace Ginecologia.Modules.Autorizacion.Application;

public interface IRolePermissionRepository
{
    Task<bool> HasPermissionAsync(string role, string permissionKey, CancellationToken ct = default);
    Task<IReadOnlyList<PermissionDefinition>> ListDefinitionsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<RolePermissionGrant>> ListGrantsAsync(CancellationToken ct = default);
    Task GrantAsync(string role, string permissionKey, CancellationToken ct = default);
    Task RevokeAsync(string role, string permissionKey, CancellationToken ct = default);
}

public sealed record PermissionDto(string Key, string Descripcion, IReadOnlyList<string> RolesConAcceso);
