using Ginecologia.Modules.HistoriaClinica.Application;
using Ginecologia.Modules.HistoriaClinica.Domain;
using Ginecologia.Modules.Pacientes.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ginecologia.Modules.HistoriaClinica.Api;

public sealed record CreateClinicalEntryRequest(Guid PatientId, ClinicalEntryType Tipo, DateOnly Fecha, string Motivo, string Hallazgos, string? Diagnostico, string? Indicaciones);
public sealed record UpdateClinicalEntryRequest(ClinicalEntryType Tipo, DateOnly Fecha, string Motivo, string Hallazgos, string? Diagnostico, string? Indicaciones);

public static class HistoriaClinicaEndpoints
{
    public static IEndpointRouteBuilder MapHistoriaClinicaEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var historiaClinica = endpoints.MapGroup("/api/historia-clinica").RequireAuthorization();
        historiaClinica.MapGet("/", ListAsync);
        historiaClinica.MapGet("/{id:guid}", GetAsync);
        historiaClinica.MapGet("/pacientes/{patientId:guid}/exportar-pdf", ExportPdfAsync);
        historiaClinica.MapPost("/", CreateAsync).RequireAuthorization("RequireClinicalStaff");
        historiaClinica.MapPut("/{id:guid}", UpdateAsync).RequireAuthorization("RequireClinicalStaff");
        return endpoints;
    }

    private static async Task<IResult> ListAsync(Guid patientId, ClinicalEntryType? tipo, int page, int pageSize, ListClinicalEntriesUseCase useCase, CancellationToken cancellationToken)
    {
        try { return Results.Ok(await useCase.ExecuteAsync(patientId, tipo, page, pageSize, cancellationToken)); }
        catch (ArgumentException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }

    private static async Task<IResult> GetAsync(Guid id, GetClinicalEntryUseCase useCase, CancellationToken cancellationToken)
    {
        var entry = await useCase.ExecuteAsync(id, cancellationToken);
        return entry is null ? Results.NotFound() : Results.Ok(entry);
    }

    private static async Task<IResult> ExportPdfAsync(Guid patientId, ExportClinicalHistoryPdfUseCase useCase, GetPatientUseCase getPatient, CancellationToken cancellationToken)
    {
        try
        {
            var patient = await getPatient.ExecuteAsync(patientId, cancellationToken) ?? throw new ArgumentException("El paciente indicado no existe.", nameof(patientId));
            var file = await useCase.ExecuteAsync(patientId, cancellationToken);
            return Results.File(file, "application/pdf", ExportClinicalHistoryPdfUseCase.SuggestedFileName(patient.NombreCompleto));
        }
        catch (ArgumentException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }

    private static async Task<IResult> CreateAsync(CreateClinicalEntryRequest request, HttpContext context, CreateClinicalEntryUseCase useCase, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(context.User.FindFirst("sub")?.Value, out var registradoPorId)) return Results.Unauthorized();
        try
        {
            var entry = await useCase.ExecuteAsync(new CreateClinicalEntryCommand(request.PatientId, request.Tipo, request.Fecha, request.Motivo, request.Hallazgos, request.Diagnostico, request.Indicaciones, registradoPorId), cancellationToken);
            return Results.Created($"/api/historia-clinica/{entry.Id}", entry);
        }
        catch (ArgumentException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }

    private static async Task<IResult> UpdateAsync(Guid id, UpdateClinicalEntryRequest request, UpdateClinicalEntryUseCase useCase, CancellationToken cancellationToken)
    {
        try { return Results.Ok(await useCase.ExecuteAsync(id, new UpdateClinicalEntryCommand(request.Tipo, request.Fecha, request.Motivo, request.Hallazgos, request.Diagnostico, request.Indicaciones), cancellationToken)); }
        catch (ClinicalEntryNotFoundException) { return Results.NotFound(); }
        catch (ArgumentException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }
}
