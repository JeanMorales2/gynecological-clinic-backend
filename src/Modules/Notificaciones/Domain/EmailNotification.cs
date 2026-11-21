namespace Ginecologia.Modules.Notificaciones.Domain;

public enum EmailNotificationStatus { Pendiente, Enviado, Fallido }

public sealed class EmailNotification
{
    private EmailNotification() { }

    private EmailNotification(Guid id, string destinatario, string asunto, string cuerpo)
    {
        Id = id;
        Destinatario = RequiredEmail(destinatario);
        Asunto = RequiredText(asunto, nameof(asunto), 200);
        Cuerpo = RequiredText(cuerpo, nameof(cuerpo), 5000);
        Estado = EmailNotificationStatus.Pendiente;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Destinatario { get; private set; } = string.Empty;
    public string Asunto { get; private set; } = string.Empty;
    public string Cuerpo { get; private set; } = string.Empty;
    public EmailNotificationStatus Estado { get; private set; }
    public int IntentosCount { get; private set; }
    public string? UltimoError { get; private set; }
    public DateTime? EnviadoAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static EmailNotification Create(string destinatario, string asunto, string cuerpo) => new(Guid.NewGuid(), destinatario, asunto, cuerpo);

    public void MarcarEnviado()
    {
        Estado = EmailNotificationStatus.Enviado;
        EnviadoAtUtc = DateTime.UtcNow;
    }

    public void MarcarFallido(string error)
    {
        Estado = EmailNotificationStatus.Fallido;
        IntentosCount++;
        var message = error ?? string.Empty;
        UltimoError = message.Length > 1000 ? message[..1000] : message;
    }

    private static string RequiredEmail(string? value)
    {
        var email = value?.Trim();
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("El destinatario debe ser un correo electrónico válido.", nameof(value));
        return email;
    }

    private static string RequiredText(string? value, string name, int max)
    {
        var text = value?.Trim();
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("El campo es obligatorio.", name);
        if (text.Length > max) throw new ArgumentException($"El campo no puede superar {max} caracteres.", name);
        return text;
    }
}
