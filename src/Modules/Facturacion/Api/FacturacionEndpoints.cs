using Ginecologia.Modules.Facturacion.Application;
using Ginecologia.Modules.Facturacion.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
namespace Ginecologia.Modules.Facturacion.Api;
public sealed record InvoiceItemRequest(string Concepto, int Cantidad, decimal PrecioUnitario);
public sealed record CreateInvoiceRequest(Guid PatientId, Guid? AppointmentId, IReadOnlyList<InvoiceItemRequest>? Items);
public sealed record MarkInvoicePaidRequest(string MetodoPago);
public static class FacturacionEndpoints
{
    public static IEndpointRouteBuilder MapFacturacionEndpoints(this IEndpointRouteBuilder endpoints) { var group = endpoints.MapGroup("/api/facturacion").RequireAuthorization(); group.MapGet("/", ListAsync); group.MapGet("/{id:guid}", GetAsync); group.MapPost("/", CreateAsync).RequireAuthorization("RequireBillingStaff"); group.MapPost("/{id:guid}/pagar", PayAsync).RequireAuthorization("RequireBillingStaff"); group.MapPost("/{id:guid}/anular", VoidAsync).RequireAuthorization("RequireBillingStaff"); return endpoints; }
    private static async Task<IResult> ListAsync(Guid? patientId, InvoiceStatus? estado, int page, int pageSize, ListInvoicesUseCase useCase, CancellationToken ct) { try { return Results.Ok(await useCase.ExecuteAsync(patientId, estado, page == 0 ? 1 : page, pageSize == 0 ? 20 : pageSize, ct)); } catch (ArgumentException ex) { return Results.BadRequest(new { message = ex.Message }); } }
    private static async Task<IResult> GetAsync(Guid id, GetInvoiceUseCase useCase, CancellationToken ct) { var invoice = await useCase.ExecuteAsync(id, ct); return invoice is null ? Results.NotFound() : Results.Ok(invoice); }
    private static async Task<IResult> CreateAsync(CreateInvoiceRequest request, HttpContext context, CreateInvoiceUseCase useCase, CancellationToken ct) { if (!Guid.TryParse(context.User.FindFirst("sub")?.Value, out var userId)) return Results.Unauthorized(); try { var items = request.Items?.Select(x => new InvoiceItemInput(x.Concepto, x.Cantidad, x.PrecioUnitario)).ToList() ?? []; var invoice = await useCase.ExecuteAsync(new(request.PatientId, request.AppointmentId, items, userId), ct); return Results.Created($"/api/facturacion/{invoice.Id}", invoice); } catch (ArgumentException ex) { return Results.BadRequest(new { message = ex.Message }); } }
    private static async Task<IResult> PayAsync(Guid id, MarkInvoicePaidRequest request, MarkInvoicePaidUseCase useCase, CancellationToken ct) { try { return Results.Ok(await useCase.ExecuteAsync(id, request.MetodoPago, ct)); } catch (InvoiceNotFoundException) { return Results.NotFound(); } catch (ArgumentException ex) { return Results.BadRequest(new { message = ex.Message }); } catch (InvalidOperationException ex) { return Results.BadRequest(new { message = ex.Message }); } }
    private static async Task<IResult> VoidAsync(Guid id, VoidInvoiceUseCase useCase, CancellationToken ct) { try { return Results.Ok(await useCase.ExecuteAsync(id, ct)); } catch (InvoiceNotFoundException) { return Results.NotFound(); } catch (InvalidOperationException ex) { return Results.BadRequest(new { message = ex.Message }); } }
}
