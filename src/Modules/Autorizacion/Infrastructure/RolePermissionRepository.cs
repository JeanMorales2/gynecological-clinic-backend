using Ginecologia.Modules.Autorizacion.Application;
using Ginecologia.Modules.Autorizacion.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.Autorizacion.Infrastructure;

public sealed class RolePermissionRepository(AutorizacionDbContext dbContext) : IRolePermissionRepository
{
    public Task<bool> HasPermissionAsync(string role, string permissionKey, CancellationToken ct = default) =>
        dbContext.RolePermissionGrants.AnyAsync(grant => grant.Role == role && grant.PermissionKey == permissionKey, ct);

    public async Task<IReadOnlyList<PermissionDefinition>> ListDefinitionsAsync(CancellationToken ct = default) =>
        await dbContext.PermissionDefinitions.AsNoTracking().OrderBy(definition => definition.Key).ToListAsync(ct);

    public async Task<IReadOnlyList<RolePermissionGrant>> ListGrantsAsync(CancellationToken ct = default) =>
        await dbContext.RolePermissionGrants.AsNoTracking().ToListAsync(ct);

    public async Task GrantAsync(string role, string permissionKey, CancellationToken ct = default)
    {
        if (await dbContext.RolePermissionGrants.AnyAsync(grant => grant.Role == role && grant.PermissionKey == permissionKey, ct))
            return;

        dbContext.RolePermissionGrants.Add(RolePermissionGrant.Create(role, permissionKey));
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task RevokeAsync(string role, string permissionKey, CancellationToken ct = default)
    {
        var grant = await dbContext.RolePermissionGrants.FindAsync([role, permissionKey], ct);
        if (grant is null)
            return;

        dbContext.RolePermissionGrants.Remove(grant);
        await dbContext.SaveChangesAsync(ct);
    }
}
