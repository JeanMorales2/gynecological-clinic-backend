using Ginecologia.Modules.Pacientes.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ginecologia.Modules.Pacientes.Api;

public sealed record CreatePatientRequest(string NombreCompleto, DateOnly FechaNacimiento, string Telefono, string? Email, string? DocumentoIdentidad, string? Alergias, string? AntecedentesRelevantes);
public sealed record UpdatePatientRequest(string NombreCompleto, DateOnly FechaNacimiento, string Telefono, string? Email, string? DocumentoIdentidad, string? Alergias, string? AntecedentesRelevantes);

public static class PacientesEndpoints
{
    public static IEndpointRouteBuilder MapPacientesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var pacientes = endpoints.MapGroup("/api/pacientes").RequireAuthorization();
        pacientes.MapGet("/", ListAsync);
        pacientes.MapGet("/{id:guid}", GetAsync);
        pacientes.MapPost("/", CreateAsync);
        pacientes.MapPut("/{id:guid}", UpdateAsync);
        pacientes.MapPost("/{id:guid}/desactivar", DeactivateAsync);
        return endpoints;
    }

    private static Task<PagedResult<PatientDto>> ListAsync(string? search, int page, int pageSize, ListPatientsUseCase useCase, CancellationToken cancellationToken) =>
        useCase.ExecuteAsync(search, page, pageSize, cancellationToken);

    private static async Task<IResult> GetAsync(Guid id, GetPatientUseCase useCase, CancellationToken cancellationToken)
    {
        var patient = await useCase.ExecuteAsync(id, cancellationToken);
        return patient is null ? Results.NotFound() : Results.Ok(patient);
    }

    private static async Task<IResult> CreateAsync(CreatePatientRequest request, CreatePatientUseCase useCase, CancellationToken cancellationToken)
    {
        try
        {
            var patient = await useCase.ExecuteAsync(new CreatePatientCommand(request.NombreCompleto, request.FechaNacimiento, request.Telefono, request.Email, request.DocumentoIdentidad, request.Alergias, request.AntecedentesRelevantes), cancellationToken);
            return Results.Created($"/api/pacientes/{patient.Id}", patient);
        }
        catch (ArgumentException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }

    private static async Task<IResult> UpdateAsync(Guid id, UpdatePatientRequest request, UpdatePatientUseCase useCase, CancellationToken cancellationToken)
    {
        try
        {
            var patient = await useCase.ExecuteAsync(id, new UpdatePatientCommand(request.NombreCompleto, request.FechaNacimiento, request.Telefono, request.Email, request.DocumentoIdentidad, request.Alergias, request.AntecedentesRelevantes), cancellationToken);
            return Results.Ok(patient);
        }
        catch (PatientNotFoundException) { return Results.NotFound(); }
        catch (ArgumentException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }

    private static async Task<IResult> DeactivateAsync(Guid id, DeactivatePatientUseCase useCase, CancellationToken cancellationToken)
    {
        try { return Results.Ok(await useCase.ExecuteAsync(id, cancellationToken)); }
        catch (PatientNotFoundException) { return Results.NotFound(); }
    }
}
