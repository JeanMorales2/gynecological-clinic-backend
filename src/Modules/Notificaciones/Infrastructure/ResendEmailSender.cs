using System.Net.Http.Json;
using Ginecologia.Modules.Notificaciones.Application;

namespace Ginecologia.Modules.Notificaciones.Infrastructure;

public sealed class ResendEmailSender(HttpClient httpClient, ResendOptions options) : IEmailSender
{
    public async Task SendAsync(string destinatario, string asunto, string cuerpoHtml, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(options.ApiKey)) throw new InvalidOperationException("Resend no está configurado.");
        if (string.IsNullOrWhiteSpace(options.FromEmail)) throw new InvalidOperationException("El remitente de Resend no está configurado.");

        var from = string.IsNullOrWhiteSpace(options.FromName) ? options.FromEmail : $"{options.FromName} <{options.FromEmail}>";
        var payload = new { from, to = new[] { destinatario }, subject = asunto, html = cuerpoHtml };

        using var response = await httpClient.PostAsJsonAsync("emails", payload, ct);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Resend respondió {(int)response.StatusCode}: {body}");
        }
    }
}
