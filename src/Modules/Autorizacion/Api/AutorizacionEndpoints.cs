using Ginecologia.Modules.Autorizacion.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ginecologia.Modules.Autorizacion.Api;

public sealed record SetPermissionGrantRequest(bool Otorgado);
public sealed record CreateRolRequest(string Nombre);

public static class AutorizacionEndpoints
{
    public static IEndpointRouteBuilder MapAutorizacionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var permissions = endpoints.MapGroup("/api/permisos").RequireAuthorization("RequireAdmin");
        permissions.MapGet("/", ListAsync);
        permissions.MapPut("/{role}/{permissionKey}", SetGrantAsync);

        var roles = endpoints.MapGroup("/api/roles").RequireAuthorization("RequireAdmin");
        roles.MapGet("/", ListRolesAsync);
        roles.MapPost("/", CreateRolAsync);
        roles.MapDelete("/{nombre}", DeleteRolAsync);
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

    private static async Task<IResult> ListRolesAsync(ListRolesUseCase useCase, CancellationToken ct) =>
        Results.Ok(await useCase.ExecuteAsync(ct));

    private static async Task<IResult> CreateRolAsync(CreateRolRequest request, CreateRolUseCase useCase, CancellationToken ct)
    {
        try { return Results.Created($"/api/roles/{request.Nombre}", await useCase.ExecuteAsync(request.Nombre, ct)); }
        catch (RolConflictException exception) { return Results.Conflict(new { message = exception.Message }); }
        catch (ArgumentException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }

    private static async Task<IResult> DeleteRolAsync(string nombre, DeleteRolUseCase useCase, CancellationToken ct)
    {
        try { await useCase.ExecuteAsync(nombre, ct); return Results.NoContent(); }
        catch (RolNotFoundException) { return Results.NotFound(); }
        catch (RolProtegidoException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }
}
