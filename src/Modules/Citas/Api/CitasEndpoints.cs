using Ginecologia.Modules.Citas.Application;
using Ginecologia.Modules.Citas.Domain;
using Ginecologia.Modules.Usuarios.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ginecologia.Modules.Citas.Api;

public sealed record CreateAppointmentRequest(Guid PatientId, Guid ProfesionalId, DateTime FechaHoraInicio, int DuracionMinutos = 30, string? Notas = null);
public sealed record UpdateAppointmentRequest(Guid PatientId, Guid ProfesionalId, DateTime FechaHoraInicio, int DuracionMinutos, string? Notas);
public sealed record ChangeAppointmentStatusRequest(AppointmentStatus Estado);

public static class CitasEndpoints
{
    public static IEndpointRouteBuilder MapCitasEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var citas = endpoints.MapGroup("/api/citas").RequireAuthorization();
        citas.MapGet("/profesionales", ListProfesionalesAsync);
        citas.MapGet("/", ListAsync);
        citas.MapGet("/{id:guid}", GetAsync);
        citas.MapPost("/", CreateAsync);
        citas.MapPut("/{id:guid}", UpdateAsync);
        citas.MapPost("/{id:guid}/estado", ChangeStatusAsync);
        return endpoints;
    }

    private static Task<PagedAppointmentResult<AppointmentDto>> ListAsync(Guid? profesionalId, Guid? patientId, DateOnly? fecha, AppointmentStatus? estado, int page, int pageSize, ListAppointmentsUseCase useCase, CancellationToken cancellationToken) =>
        useCase.ExecuteAsync(profesionalId, patientId, fecha, estado, page, pageSize, cancellationToken);

    private static async Task<IResult> GetAsync(Guid id, GetAppointmentUseCase useCase, CancellationToken cancellationToken)
    {
        var appointment = await useCase.ExecuteAsync(id, cancellationToken);
        return appointment is null ? Results.NotFound() : Results.Ok(appointment);
    }

    private static async Task<IResult> CreateAsync(CreateAppointmentRequest request, CreateAppointmentUseCase useCase, CancellationToken cancellationToken)
    {
        try
        {
            var appointment = await useCase.ExecuteAsync(new CreateAppointmentCommand(request.PatientId, request.ProfesionalId, request.FechaHoraInicio, request.DuracionMinutos, request.Notas), cancellationToken);
            return Results.Created($"/api/citas/{appointment.Id}", appointment);
        }
        catch (ArgumentException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }

    private static async Task<IResult> UpdateAsync(Guid id, UpdateAppointmentRequest request, UpdateAppointmentUseCase useCase, CancellationToken cancellationToken)
    {
        try { return Results.Ok(await useCase.ExecuteAsync(id, new UpdateAppointmentCommand(request.PatientId, request.ProfesionalId, request.FechaHoraInicio, request.DuracionMinutos, request.Notas), cancellationToken)); }
        catch (AppointmentNotFoundException) { return Results.NotFound(); }
        catch (ArgumentException exception) { return Results.BadRequest(new { message = exception.Message }); }
        catch (InvalidOperationException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }

    private static async Task<IResult> ChangeStatusAsync(Guid id, ChangeAppointmentStatusRequest request, ChangeAppointmentStatusUseCase useCase, CancellationToken cancellationToken)
    {
        try { return Results.Ok(await useCase.ExecuteAsync(id, request.Estado, cancellationToken)); }
        catch (AppointmentNotFoundException) { return Results.NotFound(); }
        catch (InvalidOperationException exception) { return Results.BadRequest(new { message = exception.Message }); }
    }

    private static async Task<IResult> ListProfesionalesAsync(ListStaffUseCase useCase, CancellationToken cancellationToken)
    {
        var professionals = (await useCase.ExecuteAsync(cancellationToken))
            .Where(staff => staff.Activo && staff.Role == "Ginecologa")
            .Select(staff => new { id = staff.Id, nombreCompleto = staff.NombreCompleto });
        return Results.Ok(professionals);
    }
}
