using Ginecologia.Modules.Notificaciones.Application;
using Ginecologia.Modules.Notificaciones.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.Notificaciones.Infrastructure;

public sealed class EmailNotificationRepository(NotificacionesDbContext db) : IEmailNotificationRepository
{
    public Task AddAsync(EmailNotification notification, CancellationToken ct = default) => db.EmailNotifications.AddAsync(notification, ct).AsTask();
    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
    public Task<EmailNotification?> FindByIdAsync(Guid id, CancellationToken ct = default) => db.EmailNotifications.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);

    public async Task<PagedEmailNotificationResult<EmailNotification>> ListAsync(EmailNotificationListQuery query, CancellationToken ct = default)
    {
        IQueryable<EmailNotification> source = db.EmailNotifications.AsNoTracking();
        if (query.Estado is { } estado) source = source.Where(x => x.Estado == estado);
        var total = await source.CountAsync(ct);
        var items = await source.OrderByDescending(x => x.CreatedAtUtc).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);
        return new(items, total, query.Page, query.PageSize);
    }
}
