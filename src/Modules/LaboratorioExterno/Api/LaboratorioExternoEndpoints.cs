using Ginecologia.Modules.LaboratorioExterno.Application;
using Ginecologia.Modules.LaboratorioExterno.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
namespace Ginecologia.Modules.LaboratorioExterno.Api;
public sealed record CreateLabOrderRequest(Guid PatientId, string TipoEstudio, string LaboratorioNombre, DateOnly FechaSolicitud, string? Observaciones);
public sealed record ReceiveLabResultRequest(string ResultadoResumen, string? Observaciones);
public static class LaboratorioExternoEndpoints
{
    public static IEndpointRouteBuilder MapLaboratorioExternoEndpoints(this IEndpointRouteBuilder endpoints) { var group = endpoints.MapGroup("/api/laboratorio-externo").RequireAuthorization(); group.MapGet("/", ListAsync); group.MapGet("/{id:guid}", GetAsync); group.MapPost("/", CreateAsync).RequireAuthorization("RequireClinicalStaff"); group.MapPost("/{id:guid}/recibido", ReceiveAsync).RequireAuthorization("RequireClinicalStaff"); group.MapPost("/{id:guid}/cancelar", CancelAsync).RequireAuthorization("RequireClinicalStaff"); return endpoints; }
    private static async Task<IResult> ListAsync(Guid patientId, LabOrderStatus? estado, int page, int pageSize, ListLabOrdersUseCase useCase, CancellationToken ct) { try { return Results.Ok(await useCase.ExecuteAsync(patientId, estado, page == 0 ? 1 : page, pageSize == 0 ? 20 : pageSize, ct)); } catch (ArgumentException ex) { return Results.BadRequest(new { message = ex.Message }); } }
    private static async Task<IResult> GetAsync(Guid id, GetLabOrderUseCase useCase, CancellationToken ct) { var order = await useCase.ExecuteAsync(id, ct); return order is null ? Results.NotFound() : Results.Ok(order); }
    private static async Task<IResult> CreateAsync(CreateLabOrderRequest request, HttpContext context, CreateLabOrderUseCase useCase, CancellationToken ct) { if (!Guid.TryParse(context.User.FindFirst("sub")?.Value, out var userId)) return Results.Unauthorized(); try { var order = await useCase.ExecuteAsync(new(request.PatientId, request.TipoEstudio, request.LaboratorioNombre, request.FechaSolicitud, request.Observaciones, userId), ct); return Results.Created($"/api/laboratorio-externo/{order.Id}", order); } catch (ArgumentException ex) { return Results.BadRequest(new { message = ex.Message }); } }
    private static async Task<IResult> ReceiveAsync(Guid id, ReceiveLabResultRequest request, MarkLabOrderReceivedUseCase useCase, CancellationToken ct) { try { return Results.Ok(await useCase.ExecuteAsync(id, request.ResultadoResumen, request.Observaciones, ct)); } catch (LabOrderNotFoundException) { return Results.NotFound(); } catch (ArgumentException ex) { return Results.BadRequest(new { message = ex.Message }); } catch (InvalidOperationException ex) { return Results.BadRequest(new { message = ex.Message }); } }
    private static async Task<IResult> CancelAsync(Guid id, CancelLabOrderUseCase useCase, CancellationToken ct) { try { return Results.Ok(await useCase.ExecuteAsync(id, ct)); } catch (LabOrderNotFoundException) { return Results.NotFound(); } catch (InvalidOperationException ex) { return Results.BadRequest(new { message = ex.Message }); } }
}
