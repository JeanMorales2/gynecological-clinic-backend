using Ginecologia.Modules.Auth.Domain;
using Ginecologia.Modules.Autorizacion.Application;

namespace Ginecologia.Modules.Auth.Application;

public sealed class AuthAccountManager(IAuthAccountRepository accounts, IPasswordService passwords, RoleExistsUseCase roleExists) : IAuthAccountManager
{
    public async Task<AuthAccountSummary> CreateAsync(CreateAuthAccountRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("La contraseña inicial es obligatoria.", nameof(request));
        if (!await roleExists.ExecuteAsync(request.Role, cancellationToken))
            throw new ArgumentException("El rol no es válido.", nameof(request));

        var email = AuthAccount.NormalizeEmail(request.Email);
        if (await accounts.FindByEmailAsync(email, cancellationToken) is not null)
            throw new AuthAccountConflictException(email);

        var unhashedAccount = AuthAccount.Create(email, "pending", request.Role);
        var account = AuthAccount.Create(email, passwords.Hash(unhashedAccount, request.Password), request.Role);
        await accounts.AddAsync(account, cancellationToken);
        await accounts.SaveChangesAsync(cancellationToken);
        return new AuthAccountSummary(account.Id, account.Email, account.Role);
    }

    public async Task ChangeRoleAsync(Guid accountId, string role, CancellationToken cancellationToken = default)
    {
        if (!await roleExists.ExecuteAsync(role, cancellationToken)) throw new ArgumentException("El rol no es válido.", nameof(role));
        var account = await accounts.FindByIdAsync(accountId, cancellationToken)
            ?? throw new KeyNotFoundException("No existe la cuenta de autenticación.");
        account.ChangeRole(role);
        await accounts.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var account = await accounts.FindByIdAsync(accountId, cancellationToken)
            ?? throw new KeyNotFoundException("No existe la cuenta de autenticación.");
        account.Deactivate();
        await accounts.SaveChangesAsync(cancellationToken);
    }

    public async Task ActivateAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var account = await accounts.FindByIdAsync(accountId, cancellationToken)
            ?? throw new KeyNotFoundException("No existe la cuenta de autenticación.");
        account.Activate();
        await accounts.SaveChangesAsync(cancellationToken);
    }
}
