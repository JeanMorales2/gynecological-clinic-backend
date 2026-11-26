using System.Text.RegularExpressions;
using Ginecologia.Modules.Auth.Application;
using Ginecologia.Modules.Auth.Domain;
using Ginecologia.Modules.Notificaciones.Application;
using Ginecologia.Modules.Notificaciones.Domain;
using Xunit;

namespace Ginecologia.Modules.Tests.Auth;

internal sealed class FakePasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly Dictionary<string, PasswordResetToken> _tokens = [];
    public Task AddAsync(PasswordResetToken token, CancellationToken cancellationToken = default) { _tokens[token.TokenHash] = token; return Task.CompletedTask; }
    public Task<PasswordResetToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        Task.FromResult(_tokens.GetValueOrDefault(tokenHash));
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}

internal sealed class FakeEmailNotificationRepository : IEmailNotificationRepository
{
    public Task AddAsync(EmailNotification notification, CancellationToken ct = default) => Task.CompletedTask;
    public Task SaveChangesAsync(CancellationToken ct = default) => Task.CompletedTask;
    public Task<EmailNotification?> FindByIdAsync(Guid id, CancellationToken ct = default) => Task.FromResult<EmailNotification?>(null);
    public Task<PagedEmailNotificationResult<EmailNotification>> ListAsync(EmailNotificationListQuery query, CancellationToken ct = default) =>
        throw new NotImplementedException();
}

internal sealed class RecordingEmailSender : IEmailSender
{
    public readonly List<(string Destinatario, string Asunto, string CuerpoHtml)> Enviados = [];
    public Task SendAsync(string destinatario, string asunto, string cuerpoHtml, CancellationToken ct = default)
    {
        Enviados.Add((destinatario, asunto, cuerpoHtml));
        return Task.CompletedTask;
    }
}

public class PasswordResetUseCasesTests
{
    private static (RequestPasswordResetUseCase RequestUseCase, ResetPasswordUseCase ResetUseCase, FakeAuthAccountRepository Accounts, RecordingEmailSender EmailSender, AuthAccount Account) CrearEscenario()
    {
        var passwords = new FakePasswordService();
        var accounts = new FakeAuthAccountRepository();
        var account = AuthAccount.Create("doctora@clinica.com", "hash-inicial", AuthRoles.Ginecologa);
        accounts.Seed(account);

        var resetTokens = new FakePasswordResetTokenRepository();
        var emailSender = new RecordingEmailSender();
        var sendEmailNotification = new SendEmailNotificationUseCase(new FakeEmailNotificationRepository(), emailSender);
        var requestUseCase = new RequestPasswordResetUseCase(accounts, resetTokens, sendEmailNotification, new AuthFrontendOptions("https://clinica.local"));
        var resetUseCase = new ResetPasswordUseCase(accounts, resetTokens, passwords);
        return (requestUseCase, resetUseCase, accounts, emailSender, account);
    }

    private static string ExtraerToken(string cuerpoHtml)
    {
        var match = Regex.Match(cuerpoHtml, @"token=([^""&]+)");
        Assert.True(match.Success, "El cuerpo del correo no contiene un token.");
        return Uri.UnescapeDataString(match.Groups[1].Value);
    }

    [Fact]
    public async Task FlujoCompleto_SolicitarYResetear_CambiaLaContrasena()
    {
        var (requestUseCase, resetUseCase, _, emailSender, account) = CrearEscenario();

        await requestUseCase.ExecuteAsync(account.Email);

        Assert.Single(emailSender.Enviados);
        var token = ExtraerToken(emailSender.Enviados[0].CuerpoHtml);

        await resetUseCase.ExecuteAsync(token, "ClaveNueva123!");

        Assert.Equal("hash:ClaveNueva123!", account.PasswordHash);
    }

    [Fact]
    public async Task ResetUseCase_ConTokenYaUsado_Lanza()
    {
        var (requestUseCase, resetUseCase, _, emailSender, account) = CrearEscenario();
        await requestUseCase.ExecuteAsync(account.Email);
        var token = ExtraerToken(emailSender.Enviados[0].CuerpoHtml);
        await resetUseCase.ExecuteAsync(token, "ClaveNueva123!");

        await Assert.ThrowsAsync<InvalidResetTokenException>(() => resetUseCase.ExecuteAsync(token, "OtraClave456!"));
    }

    [Fact]
    public async Task ResetUseCase_ConTokenInexistente_Lanza()
    {
        var (_, resetUseCase, _, _, _) = CrearEscenario();

        await Assert.ThrowsAsync<InvalidResetTokenException>(() => resetUseCase.ExecuteAsync("token-invalido", "ClaveNueva123!"));
    }

    [Fact]
    public async Task RequestUseCase_ConEmailInexistente_NoEnviaCorreoNiLanza()
    {
        var (requestUseCase, _, _, emailSender, _) = CrearEscenario();

        await requestUseCase.ExecuteAsync("no-existe@clinica.com");

        Assert.Empty(emailSender.Enviados);
    }

    [Theory]
    [InlineData("corta")]
    [InlineData("")]
    public async Task ResetUseCase_ConContrasenaNuevaMuyCorta_Lanza(string nuevaPassword)
    {
        var (requestUseCase, resetUseCase, _, emailSender, account) = CrearEscenario();
        await requestUseCase.ExecuteAsync(account.Email);
        var token = ExtraerToken(emailSender.Enviados[0].CuerpoHtml);

        await Assert.ThrowsAsync<ArgumentException>(() => resetUseCase.ExecuteAsync(token, nuevaPassword));
    }
}
