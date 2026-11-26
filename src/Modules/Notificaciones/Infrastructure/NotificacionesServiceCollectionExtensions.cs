using System.Net.Http.Headers;
using Ginecologia.Modules.Notificaciones.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ginecologia.Modules.Notificaciones.Infrastructure;

public static class NotificacionesServiceCollectionExtensions
{
    public static IServiceCollection AddNotificacionesModule(this IServiceCollection services, string connectionString, ResendOptions resendOptions)
    {
        services.AddSingleton(resendOptions);
        services.AddHttpClient<ResendEmailSender>((sp, client) =>
        {
            client.BaseAddress = new Uri("https://api.resend.com/");
            if (!string.IsNullOrWhiteSpace(resendOptions.ApiKey))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", resendOptions.ApiKey);
        });
        services.AddScoped<IEmailSender>(sp => sp.GetRequiredService<ResendEmailSender>());
        services.AddDbContext<NotificacionesDbContext>(x => x.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "notificaciones")));
        services.AddScoped<IEmailNotificationRepository, EmailNotificationRepository>();
        services.AddScoped<SendEmailNotificationUseCase>();
        services.AddScoped<GetEmailNotificationUseCase>();
        services.AddScoped<ListEmailNotificationsUseCase>();
        return services;
    }
}
