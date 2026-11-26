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

public interface IRolRepository
{
    Task<IReadOnlyList<Rol>> ListAsync(CancellationToken ct = default);
    Task<Rol?> FindByNombreAsync(string nombre, CancellationToken ct = default);
    Task AddAsync(Rol rol, CancellationToken ct = default);
    void Remove(Rol rol);
    Task SaveChangesAsync(CancellationToken ct = default);
}

public sealed record PermissionDto(string Key, string Descripcion, IReadOnlyList<string> RolesConAcceso);
public sealed record RolDto(string Nombre, bool EsProtegido)
{
    public static RolDto From(Rol rol) => new(rol.Nombre, rol.EsProtegido);
}

public sealed class RolConflictException(string nombre) : InvalidOperationException($"Ya existe el rol {nombre}.");
public sealed class RolNotFoundException(string nombre) : KeyNotFoundException($"No existe el rol {nombre}.");
public sealed class RolProtegidoException(string nombre) : InvalidOperationException($"El rol {nombre} es un rol protegido del sistema y no se puede eliminar.");
