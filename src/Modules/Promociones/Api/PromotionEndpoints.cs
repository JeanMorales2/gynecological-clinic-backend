using Ginecologia.Modules.Promociones.Application;
using Ginecologia.Modules.Promociones.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
namespace Ginecologia.Modules.Promociones.Api;
public static class PromotionEndpoints
{
 public static IEndpointRouteBuilder MapPromotionEndpoints(this IEndpointRouteBuilder endpoints) { var group = endpoints.MapGroup("/api/promociones").RequireAuthorization(); group.MapGet("/", ListAsync); group.MapGet("/{id:guid}", GetAsync); group.MapPost("/{id:guid}/descartar", DiscardAsync).RequireAuthorization("RequireInventoryStaff"); group.MapPost("/generar-ahora", GenerateNowAsync).RequireAuthorization("RequireInventoryStaff"); return endpoints; }
 private static async Task<IResult> ListAsync(Guid? patientId, PromotionStatus? estado, int page, int pageSize, ListPromotionsUseCase uc, CancellationToken ct) { try { return Results.Ok(await uc.ExecuteAsync(patientId, estado, page == 0 ? 1 : page, pageSize == 0 ? 20 : pageSize, ct)); } catch (ArgumentException ex) { return Results.BadRequest(new { message = ex.Message }); } }
 private static async Task<IResult> GetAsync(Guid id, GetPromotionUseCase uc, CancellationToken ct) { var promotion = await uc.ExecuteAsync(id, ct); return promotion is null ? Results.NotFound() : Results.Ok(promotion); }
 private static async Task<IResult> DiscardAsync(Guid id, DiscardPromotionUseCase uc, CancellationToken ct) { try { return Results.Ok(await uc.ExecuteAsync(id, ct)); } catch (PromotionNotFoundException) { return Results.NotFound(); } catch (InvalidOperationException ex) { return Results.BadRequest(new { message = ex.Message }); } }
 private static async Task<IResult> GenerateNowAsync(GenerateBirthdayPromotionsUseCase uc, CancellationToken ct) => Results.Ok(new { generated = await uc.ExecuteAsync(DateOnly.FromDateTime(DateTime.UtcNow), ct) });
}
