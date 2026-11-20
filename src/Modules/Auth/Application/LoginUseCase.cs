namespace Ginecologia.Modules.Auth.Application;

public sealed class LoginUseCase(IAuthAccountRepository accounts, IPasswordService passwords, ITokenIssuer tokens)
{
    public async Task<LoginResult?> ExecuteAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return null;

        var account = await accounts.FindByEmailAsync(email, cancellationToken);
        if (account is null || !account.IsActive || !passwords.Verify(account, password)) return null;

        var token = tokens.Issue(account);
        return new LoginResult(token.AccessToken, token.ExpiresAtUtc, PublicAuthUser.From(account));
    }
}
