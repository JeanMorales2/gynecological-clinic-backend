using System.Security.Claims;
using Ginecologia.Modules.Autorizacion.Application;
using Microsoft.AspNetCore.Authorization;

namespace Ginecologia.Modules.Autorizacion.Api;

public sealed class PermissionAuthorizationHandler(IsRolePermittedUseCase isRolePermitted) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        // El JWT emite el claim de rol con el nombre corto "role" (ver JwtTokenIssuer.cs), no con
        // el URI largo de ClaimTypes.Role. FindFirst(ClaimTypes.Role) no respeta el RoleClaimType
        // configurado en TokenValidationParameters (eso solo afecta a IsInRole/RequireRole), así
        // que hay que leer el claim con el mismo nombre que la identidad usa realmente como rol.
        var identity = context.User.Identity as ClaimsIdentity;
        var role = context.User.FindFirst(identity?.RoleClaimType ?? ClaimTypes.Role)?.Value;
        if (role is null)
        {
            context.Fail();
            return;
        }

        if (await isRolePermitted.ExecuteAsync(role, requirement.PermissionKey))
            context.Succeed(requirement);
    }
}
