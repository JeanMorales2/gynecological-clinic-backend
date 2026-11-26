using System.Security.Cryptography;
using System.Text;
using Ginecologia.Modules.Auth.Domain;
using Ginecologia.Modules.Notificaciones.Application;

namespace Ginecologia.Modules.Auth.Application;

public sealed class RequestPasswordResetUseCase(IAuthAccountRepository accounts, IPasswordResetTokenRepository resetTokens, SendEmailNotificationUseCase sendEmailNotification, AuthFrontendOptions frontendOptions)
{
    private static readonly TimeSpan ValidFor = TimeSpan.FromMinutes(30);

    // Siempre responde igual exista o no la cuenta (no filtrar si un email esta registrado).
    public async Task ExecuteAsync(string email, CancellationToken cancellationToken = default)
    {
        AuthAccount? account;
        try { account = await accounts.FindByEmailAsync(email, cancellationToken); }
        catch (ArgumentException) { return; }
        if (account is null || !account.IsActive) return;

        var rawToken = GenerateToken();
        var resetToken = PasswordResetToken.Create(account.Id, Hash(rawToken), ValidFor);
        await resetTokens.AddAsync(resetToken, cancellationToken);
        await resetTokens.SaveChangesAsync(cancellationToken);

        var link = $"{frontendOptions.BaseUrl}/login/resetear?token={Uri.EscapeDataString(rawToken)}";
        var body = $"<p>Recibimos una solicitud para restablecer tu contraseña.</p>" +
                   $"<p><a href=\"{link}\">Hacé clic acá para elegir una nueva contraseña</a>. El enlace vence en 30 minutos.</p>" +
                   "<p>Si no pediste este cambio, podés ignorar este correo.</p>";
        await sendEmailNotification.ExecuteAsync(account.Email, "Restablecer tu contraseña", body, cancellationToken);
    }

    private static string GenerateToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).Replace('+', '-').Replace('/', '_').TrimEnd('=');

    internal static string Hash(string token) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}

public sealed class ResetPasswordUseCase(IAuthAccountRepository accounts, IPasswordResetTokenRepository resetTokens, IPasswordService passwords)
{
    private const int MinPasswordLength = 8;

    public async Task ExecuteAsync(string token, string newPassword, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < MinPasswordLength)
            throw new ArgumentException($"La nueva contraseña debe tener al menos {MinPasswordLength} caracteres.", nameof(newPassword));

        var resetToken = await resetTokens.FindByTokenHashAsync(RequestPasswordResetUseCase.Hash(token), cancellationToken);
        if (resetToken is null || !resetToken.IsValid(DateTime.UtcNow)) throw new InvalidResetTokenException();

        var account = await accounts.FindByIdAsync(resetToken.AccountId, cancellationToken) ?? throw new InvalidResetTokenException();
        account.SetPasswordHash(passwords.Hash(account, newPassword));
        resetToken.MarkUsed();
        await resetTokens.SaveChangesAsync(cancellationToken);
    }
}

public sealed record AuthFrontendOptions(string BaseUrl);
