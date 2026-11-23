using Ginecologia.Modules.Autorizacion.Application;

namespace Ginecologia.Modules.Auth.Application;

public sealed class GetCurrentUserUseCase(IAuthAccountRepository accounts, ListGrantedPermissionKeysUseCase listGrantedPermissionKeys)
{
    public async Task<PublicAuthUser?> ExecuteAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var account = await accounts.FindByIdAsync(accountId, cancellationToken);
        if (account is null) return null;

        var permisos = await listGrantedPermissionKeys.ExecuteAsync(account.Role, cancellationToken);
        return new PublicAuthUser(account.Id, account.Email, account.Role, permisos);
    }
}
