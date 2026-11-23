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
    public static IEndpointRouteBuilder MapLaboratorioExternoEndpoints(this IEndpointRouteBuilder endpoints) { var group = endpoints.MapGroup("/api/laboratorio-externo").RequireAuthorization(); group.MapGet("/", ListAsync); group.MapGet("/{id:guid}", GetAsync); group.MapPost("/", CreateAsync).RequireAuthorization("RequireClinicalStaff"); group.MapPost("/{id:guid}/recibido", ReceiveAsync).RequireAuthorization("RequireClinicalStaff"); group.MapPost("/{id:guid}/cancelar", CancelAsync).RequireAuthorization("RequireClinicalStaff"); group.MapPost("/{id:guid}/adjuntos", UploadAttachmentAsync).RequireAuthorization("RequireClinicalStaff"); group.MapGet("/{id:guid}/adjuntos", ListAttachmentsAsync); group.MapGet("/adjuntos/{attachmentId:guid}/contenido", GetAttachmentContentAsync); group.MapDelete("/adjuntos/{attachmentId:guid}", DeleteAttachmentAsync).RequireAuthorization("RequireClinicalStaff"); return endpoints; }
    private static async Task<IResult> ListAsync(Guid patientId, LabOrderStatus? estado, int page, int pageSize, ListLabOrdersUseCase useCase, CancellationToken ct) { try { return Results.Ok(await useCase.ExecuteAsync(patientId, estado, page == 0 ? 1 : page, pageSize == 0 ? 20 : pageSize, ct)); } catch (ArgumentException ex) { return Results.BadRequest(new { message = ex.Message }); } }
    private static async Task<IResult> GetAsync(Guid id, GetLabOrderUseCase useCase, CancellationToken ct) { var order = await useCase.ExecuteAsync(id, ct); return order is null ? Results.NotFound() : Results.Ok(order); }
    private static async Task<IResult> CreateAsync(CreateLabOrderRequest request, HttpContext context, CreateLabOrderUseCase useCase, CancellationToken ct) { if (!Guid.TryParse(context.User.FindFirst("sub")?.Value, out var userId)) return Results.Unauthorized(); try { var order = await useCase.ExecuteAsync(new(request.PatientId, request.TipoEstudio, request.LaboratorioNombre, request.FechaSolicitud, request.Observaciones, userId), ct); return Results.Created($"/api/laboratorio-externo/{order.Id}", order); } catch (ArgumentException ex) { return Results.BadRequest(new { message = ex.Message }); } }
    private static async Task<IResult> ReceiveAsync(Guid id, ReceiveLabResultRequest request, MarkLabOrderReceivedUseCase useCase, CancellationToken ct) { try { return Results.Ok(await useCase.ExecuteAsync(id, request.ResultadoResumen, request.Observaciones, ct)); } catch (LabOrderNotFoundException) { return Results.NotFound(); } catch (ArgumentException ex) { return Results.BadRequest(new { message = ex.Message }); } catch (InvalidOperationException ex) { return Results.BadRequest(new { message = ex.Message }); } }
    private static async Task<IResult> CancelAsync(Guid id, CancelLabOrderUseCase useCase, CancellationToken ct) { try { return Results.Ok(await useCase.ExecuteAsync(id, ct)); } catch (LabOrderNotFoundException) { return Results.NotFound(); } catch (InvalidOperationException ex) { return Results.BadRequest(new { message = ex.Message }); } }
    private static async Task<IResult> UploadAttachmentAsync(Guid id, HttpRequest request, HttpContext context, UploadLabOrderAttachmentUseCase useCase, CancellationToken ct)
    {
        if (!Guid.TryParse(context.User.FindFirst("sub")?.Value, out var userId)) return Results.Unauthorized();
        var file = (await request.ReadFormAsync(ct)).Files["archivo"];
        if (file is null) return Results.BadRequest(new { message = "Debe adjuntar un archivo." });
        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream, ct);
        try { var attachment = await useCase.ExecuteAsync(id, file.FileName, file.ContentType, stream.ToArray(), userId, ct); return Results.Created($"/api/laboratorio-externo/adjuntos/{attachment.Id}", attachment); }
        catch (LabOrderNotFoundException) { return Results.NotFound(); }
        catch (ArgumentException ex) { return Results.BadRequest(new { message = ex.Message }); }
    }
    private static async Task<IResult> ListAttachmentsAsync(Guid id, ListLabOrderAttachmentsUseCase useCase, CancellationToken ct) => Results.Ok(await useCase.ExecuteAsync(id, ct));
    private static async Task<IResult> GetAttachmentContentAsync(Guid attachmentId, GetLabOrderAttachmentContentUseCase useCase, CancellationToken ct) { var content = await useCase.ExecuteAsync(attachmentId, ct); return content is null ? Results.NotFound() : Results.File(content.Contenido, content.ContentType, content.NombreArchivo); }
    private static async Task<IResult> DeleteAttachmentAsync(Guid attachmentId, DeleteLabOrderAttachmentUseCase useCase, CancellationToken ct) { try { await useCase.ExecuteAsync(attachmentId, ct); return Results.NoContent(); } catch (KeyNotFoundException) { return Results.NotFound(); } }
}
