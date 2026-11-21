namespace Ginecologia.Modules.Notificaciones.Infrastructure;

public sealed record SmtpOptions(string? Host, int Port, string? Username, string? Password, string? FromEmail, string? FromName, bool UseStartTls);
