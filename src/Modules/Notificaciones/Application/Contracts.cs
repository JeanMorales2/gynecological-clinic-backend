using Ginecologia.Modules.Notificaciones.Domain;

namespace Ginecologia.Modules.Notificaciones.Application;

public interface IEmailNotificationRepository
{
    Task AddAsync(EmailNotification notification, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task<EmailNotification?> FindByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedEmailNotificationResult<EmailNotification>> ListAsync(EmailNotificationListQuery query, CancellationToken ct = default);
}

public interface IEmailSender
{
    Task SendAsync(string destinatario, string asunto, string cuerpoHtml, CancellationToken ct = default);
}

public sealed record EmailNotificationListQuery(EmailNotificationStatus? Estado, int Page, int PageSize);
public sealed record PagedEmailNotificationResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
public sealed record EmailNotificationDto(Guid Id, string Destinatario, string Asunto, EmailNotificationStatus Estado, int IntentosCount, string? UltimoError, DateTime? EnviadoAtUtc, DateTime CreatedAtUtc)
{
    public static EmailNotificationDto From(EmailNotification notification) => new(notification.Id, notification.Destinatario, notification.Asunto, notification.Estado, notification.IntentosCount, notification.UltimoError, notification.EnviadoAtUtc, notification.CreatedAtUtc);
}
