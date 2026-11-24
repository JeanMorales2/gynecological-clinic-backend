using Ginecologia.Modules.Notificaciones.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ginecologia.Modules.Notificaciones.Infrastructure;

public static class NotificacionesServiceCollectionExtensions
{
    public static IServiceCollection AddNotificacionesModule(this IServiceCollection services, string connectionString, SmtpOptions smtpOptions)
    {
        services.AddSingleton(smtpOptions);
        services.AddDbContext<NotificacionesDbContext>(x => x.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "notificaciones")));
        services.AddScoped<IEmailNotificationRepository, EmailNotificationRepository>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<SendEmailNotificationUseCase>();
        services.AddScoped<GetEmailNotificationUseCase>();
        services.AddScoped<ListEmailNotificationsUseCase>();
        return services;
    }
}
