using Ginecologia.Modules.Autorizacion.Application;

namespace Ginecologia.Modules.Auth.Application;

public sealed class LoginUseCase(IAuthAccountRepository accounts, IPasswordService passwords, ITokenIssuer tokens, ListGrantedPermissionKeysUseCase listGrantedPermissionKeys)
{
    public async Task<LoginResult?> ExecuteAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return null;

        var account = await accounts.FindByEmailAsync(email, cancellationToken);
        if (account is null || !account.IsActive || !passwords.Verify(account, password)) return null;

        var token = tokens.Issue(account);
        var permisos = await listGrantedPermissionKeys.ExecuteAsync(account.Role, cancellationToken);
        return new LoginResult(token.AccessToken, token.ExpiresAtUtc, new PublicAuthUser(account.Id, account.Email, account.Role, permisos));
    }
}
