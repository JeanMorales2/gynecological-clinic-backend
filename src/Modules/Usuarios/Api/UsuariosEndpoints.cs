using Ginecologia.Modules.Auth.Application;
using Ginecologia.Modules.Usuarios.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ginecologia.Modules.Usuarios.Api;

public sealed record CreateStaffRequest(string Email, string Password, string NombreCompleto, string? Telefono, string Role);
public sealed record UpdateStaffRequest(string NombreCompleto, string? Telefono, string Role);

public static class UsuariosEndpoints
{
    public static IEndpointRouteBuilder MapUsuariosEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var usuarios = endpoints.MapGroup("/api/usuarios").RequireAuthorization("RequireAdmin");
        usuarios.MapGet("/", ListAsync);
        usuarios.MapGet("/{id:guid}", GetAsync);
        usuarios.MapPost("/", CreateAsync);
        usuarios.MapPut("/{id:guid}", UpdateAsync);
        usuarios.MapPost("/{id:guid}/desactivar", DeactivateAsync);
        return endpoints;
    }

    private static async Task<IResult> ListAsync(ListStaffUseCase useCase, CancellationToken cancellationToken) =>
        Results.Ok(await useCase.ExecuteAsync(cancellationToken));

    private static async Task<IResult> GetAsync(Guid id, GetStaffUseCase useCase, CancellationToken cancellationToken)
    {
        var staff = await useCase.ExecuteAsync(id, cancellationToken);
        return staff is null ? Results.NotFound() : Results.Ok(staff);
    }

    private static async Task<IResult> CreateAsync(CreateStaffRequest request, CreateStaffUseCase useCase, CancellationToken cancellationToken)
    {
        try
        {
            var staff = await useCase.ExecuteAsync(new CreateStaffCommand(request.Email, request.Password, request.NombreCompleto, request.Telefono, request.Role), cancellationToken);
            return Results.Created($"/api/usuarios/{staff.Id}", staff);
        }
        catch (AuthAccountConflictException exception) { return Results.Conflict(new { message = exception.Message }); }
        catch (ArgumentException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }

    private static async Task<IResult> UpdateAsync(Guid id, UpdateStaffRequest request, UpdateStaffUseCase useCase, CancellationToken cancellationToken)
    {
        try { return Results.Ok(await useCase.ExecuteAsync(id, new UpdateStaffCommand(request.NombreCompleto, request.Telefono, request.Role), cancellationToken)); }
        catch (StaffNotFoundException) { return Results.NotFound(); }
        catch (ArgumentException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }

    private static async Task<IResult> DeactivateAsync(Guid id, DeactivateStaffUseCase useCase, CancellationToken cancellationToken)
    {
        try { return Results.Ok(await useCase.ExecuteAsync(id, cancellationToken)); }
        catch (StaffNotFoundException) { return Results.NotFound(); }
    }
}
