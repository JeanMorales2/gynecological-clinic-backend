namespace Ginecologia.Modules.Auth.Application;

public sealed class GetCurrentUserUseCase(IAuthAccountRepository accounts)
{
    public async Task<PublicAuthUser?> ExecuteAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var account = await accounts.FindByIdAsync(accountId, cancellationToken);
        return account is null ? null : PublicAuthUser.From(account);
    }
}
