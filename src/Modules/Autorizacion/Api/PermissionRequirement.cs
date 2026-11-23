using Microsoft.AspNetCore.Authorization;

namespace Ginecologia.Modules.Autorizacion.Api;

public sealed class PermissionRequirement(string permissionKey) : IAuthorizationRequirement
{
    public string PermissionKey { get; } = permissionKey;
}
