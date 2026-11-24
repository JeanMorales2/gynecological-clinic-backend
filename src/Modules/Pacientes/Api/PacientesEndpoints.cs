using Ginecologia.Modules.Pacientes.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ginecologia.Modules.Pacientes.Api;

public sealed record CreatePatientRequest(string NombreCompleto, DateOnly FechaNacimiento, string Telefono, string? Email, string? DocumentoIdentidad, string? Alergias, string? AntecedentesRelevantes, string? EnfermedadesCronicas, string? EtsIts, string? Vicios, string? TipoSangre, int? Gestas, int? Partos, int? Cesareas, int? Abortos);
public sealed record UpdatePatientRequest(string NombreCompleto, DateOnly FechaNacimiento, string Telefono, string? Email, string? DocumentoIdentidad, string? Alergias, string? AntecedentesRelevantes, string? EnfermedadesCronicas, string? EtsIts, string? Vicios, string? TipoSangre, int? Gestas, int? Partos, int? Cesareas, int? Abortos);
public sealed record AddFamilyMedicalHistoryRequest(string Condicion, string Parentesco);

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
        pacientes.MapPost("/{id:guid}/reactivar", ReactivateAsync);
        var antecedentesFamiliares = pacientes.MapGroup("/{patientId:guid}/antecedentes-familiares");
        antecedentesFamiliares.MapGet("/", ListFamilyMedicalHistoryAsync);
        antecedentesFamiliares.MapPost("/", AddFamilyMedicalHistoryAsync);
        antecedentesFamiliares.MapDelete("/{id:guid}", RemoveFamilyMedicalHistoryAsync);
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
            var patient = await useCase.ExecuteAsync(new CreatePatientCommand(request.NombreCompleto, request.FechaNacimiento, request.Telefono, request.Email, request.DocumentoIdentidad, request.Alergias, request.AntecedentesRelevantes, request.EnfermedadesCronicas, request.EtsIts, request.Vicios, request.TipoSangre, request.Gestas, request.Partos, request.Cesareas, request.Abortos), cancellationToken);
            return Results.Created($"/api/pacientes/{patient.Id}", patient);
        }
        catch (ArgumentException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }

    private static async Task<IResult> UpdateAsync(Guid id, UpdatePatientRequest request, UpdatePatientUseCase useCase, CancellationToken cancellationToken)
    {
        try
        {
            var patient = await useCase.ExecuteAsync(id, new UpdatePatientCommand(request.NombreCompleto, request.FechaNacimiento, request.Telefono, request.Email, request.DocumentoIdentidad, request.Alergias, request.AntecedentesRelevantes, request.EnfermedadesCronicas, request.EtsIts, request.Vicios, request.TipoSangre, request.Gestas, request.Partos, request.Cesareas, request.Abortos), cancellationToken);
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

    private static async Task<IResult> ReactivateAsync(Guid id, ReactivatePatientUseCase useCase, CancellationToken cancellationToken)
    {
        try { return Results.Ok(await useCase.ExecuteAsync(id, cancellationToken)); }
        catch (PatientNotFoundException) { return Results.NotFound(); }
    }

    private static Task<IReadOnlyList<FamilyMedicalHistoryDto>> ListFamilyMedicalHistoryAsync(Guid patientId, ListFamilyMedicalHistoryUseCase useCase, CancellationToken cancellationToken) =>
        useCase.ExecuteAsync(patientId, cancellationToken);

    private static async Task<IResult> AddFamilyMedicalHistoryAsync(Guid patientId, AddFamilyMedicalHistoryRequest request, AddFamilyMedicalHistoryUseCase useCase, CancellationToken cancellationToken)
    {
        try
        {
            var familyMedicalHistory = await useCase.ExecuteAsync(patientId, request.Condicion, request.Parentesco, cancellationToken);
            return Results.Created($"/api/pacientes/{patientId}/antecedentes-familiares/{familyMedicalHistory.Id}", familyMedicalHistory);
        }
        catch (PatientNotFoundException) { return Results.NotFound(); }
        catch (ArgumentException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }

    private static async Task<IResult> RemoveFamilyMedicalHistoryAsync(Guid id, RemoveFamilyMedicalHistoryUseCase useCase, CancellationToken cancellationToken)
    {
        try
        {
            await useCase.ExecuteAsync(id, cancellationToken);
            return Results.NoContent();
        }
        catch (FamilyMedicalHistoryNotFoundException) { return Results.NotFound(); }
    }
}
