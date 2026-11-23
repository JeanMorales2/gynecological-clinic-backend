using Ginecologia.Modules.Autorizacion.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ginecologia.Modules.Autorizacion.Api;

public sealed record SetPermissionGrantRequest(bool Otorgado);

public static class AutorizacionEndpoints
{
    public static IEndpointRouteBuilder MapAutorizacionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var permissions = endpoints.MapGroup("/api/permisos").RequireAuthorization("RequireAdmin");
        permissions.MapGet("/", ListAsync);
        permissions.MapPut("/{role}/{permissionKey}", SetGrantAsync);
        return endpoints;
    }

    private static async Task<IResult> ListAsync(ListPermissionsUseCase useCase, CancellationToken ct) =>
        Results.Ok(await useCase.ExecuteAsync(ct));

    private static async Task<IResult> SetGrantAsync(string role, string permissionKey, SetPermissionGrantRequest request, SetPermissionGrantUseCase useCase, CancellationToken ct)
    {
        try
        {
            await useCase.ExecuteAsync(role, permissionKey, request.Otorgado, ct);
            return Results.NoContent();
        }
        catch (ArgumentException exception)
        {
            return Results.BadRequest(new { message = exception.Message });
        }
    }
}
