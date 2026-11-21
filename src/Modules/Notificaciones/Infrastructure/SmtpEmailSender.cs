using Ginecologia.Modules.Notificaciones.Application;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Ginecologia.Modules.Notificaciones.Infrastructure;

public sealed class SmtpEmailSender(SmtpOptions options) : IEmailSender
{
    public async Task SendAsync(string destinatario, string asunto, string cuerpoHtml, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(options.Host)) throw new InvalidOperationException("SMTP no configurado.");
        if (string.IsNullOrWhiteSpace(options.FromEmail)) throw new InvalidOperationException("El remitente SMTP no está configurado.");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(options.FromName ?? string.Empty, options.FromEmail));
        message.To.Add(MailboxAddress.Parse(destinatario));
        message.Subject = asunto;
        message.Body = new BodyBuilder { HtmlBody = cuerpoHtml }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(options.Host, options.Port, options.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None, ct);
        if (!string.IsNullOrWhiteSpace(options.Username) && !string.IsNullOrWhiteSpace(options.Password))
            await client.AuthenticateAsync(options.Username, options.Password, ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
    }
}
