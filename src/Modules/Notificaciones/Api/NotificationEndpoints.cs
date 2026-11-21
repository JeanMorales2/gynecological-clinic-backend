using Ginecologia.Modules.Notificaciones.Application;
using Ginecologia.Modules.Notificaciones.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ginecologia.Modules.Notificaciones.Api;

public sealed record TestEmailRequest(string Destinatario);

public static class NotificationEndpoints
{
    public static IEndpointRouteBuilder MapNotificationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/notificaciones").RequireAuthorization("RequireAdmin");
        group.MapGet("/", ListAsync);
        group.MapGet("/{id:guid}", GetAsync);
        group.MapPost("/probar", TestAsync);
        return endpoints;
    }

    private static async Task<IResult> ListAsync(EmailNotificationStatus? estado, int page, int pageSize, ListEmailNotificationsUseCase useCase, CancellationToken ct)
    {
        try { return Results.Ok(await useCase.ExecuteAsync(estado, page == 0 ? 1 : page, pageSize == 0 ? 20 : pageSize, ct)); }
        catch (ArgumentException ex) { return Results.BadRequest(new { message = ex.Message }); }
    }

    private static async Task<IResult> GetAsync(Guid id, GetEmailNotificationUseCase useCase, CancellationToken ct)
    {
        var notification = await useCase.ExecuteAsync(id, ct);
        return notification is null ? Results.NotFound() : Results.Ok(notification);
    }

    private static async Task<IResult> TestAsync(TestEmailRequest request, SendEmailNotificationUseCase useCase, CancellationToken ct)
    {
        try { return Results.Ok(await useCase.ExecuteAsync(request.Destinatario, "Correo de prueba — Ginecología", "<p>Este es un correo de prueba del sistema de Ginecología.</p>", ct)); }
        catch (ArgumentException ex) { return Results.BadRequest(new { message = ex.Message }); }
    }
}
