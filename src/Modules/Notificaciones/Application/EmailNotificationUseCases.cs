using Ginecologia.Modules.Notificaciones.Domain;

namespace Ginecologia.Modules.Notificaciones.Application;

public sealed class SendEmailNotificationUseCase(IEmailNotificationRepository notifications, IEmailSender emailSender)
{
    public async Task<EmailNotificationDto> ExecuteAsync(string destinatario, string asunto, string cuerpoHtml, CancellationToken ct = default)
    {
        var notification = EmailNotification.Create(destinatario, asunto, cuerpoHtml);
        await notifications.AddAsync(notification, ct);
        try
        {
            await emailSender.SendAsync(destinatario, asunto, cuerpoHtml, ct);
            notification.MarcarEnviado();
        }
        catch (Exception ex)
        {
            notification.MarcarFallido(ex.Message);
        }

        await notifications.SaveChangesAsync(ct);
        return EmailNotificationDto.From(notification);
    }
}

public sealed class GetEmailNotificationUseCase(IEmailNotificationRepository notifications)
{
    public async Task<EmailNotificationDto?> ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var notification = await notifications.FindByIdAsync(id, ct);
        return notification is null ? null : EmailNotificationDto.From(notification);
    }
}

public sealed class ListEmailNotificationsUseCase(IEmailNotificationRepository notifications)
{
    public async Task<PagedEmailNotificationResult<EmailNotificationDto>> ExecuteAsync(EmailNotificationStatus? estado, int page, int pageSize, CancellationToken ct = default)
    {
        if (page < 1 || pageSize is < 1 or > 100) throw new ArgumentException("La paginación es inválida.");
        var result = await notifications.ListAsync(new EmailNotificationListQuery(estado, page, pageSize), ct);
        return new(result.Items.Select(EmailNotificationDto.From).ToList(), result.TotalCount, result.Page, result.PageSize);
    }
}
