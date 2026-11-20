using Ginecologia.Modules.Auth.Application;
using Ginecologia.Modules.Auth.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Ginecologia.Modules.Auth.Infrastructure;

public static class AuthSeed
{
    public static async Task SeedAdminAsync(IServiceProvider services, string? seedAdminEmail, string? seedAdminPassword, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var accounts = scope.ServiceProvider.GetRequiredService<IAuthAccountRepository>();
        if (await accounts.HasAccountsAsync(cancellationToken)) return;

        var email = seedAdminEmail ?? "admin@ginecologia.local";
        var password = seedAdminPassword;
        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("AUTH_SEED_ADMIN_PASSWORD es obligatoria cuando no hay cuentas de autenticación.");

        var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
        var account = AuthAccount.Create(email, "seed-pending", AuthRoles.Admin);
        var hashedAccount = AuthAccount.Create(email, passwordService.Hash(account, password), AuthRoles.Admin);
        await accounts.AddAsync(hashedAccount, cancellationToken);
        await accounts.SaveChangesAsync(cancellationToken);
    }
}
