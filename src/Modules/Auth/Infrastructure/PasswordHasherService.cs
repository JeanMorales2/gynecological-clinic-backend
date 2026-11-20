using Ginecologia.Modules.Auth.Application;
using Ginecologia.Modules.Auth.Domain;
using Microsoft.AspNetCore.Identity;

namespace Ginecologia.Modules.Auth.Infrastructure;

public sealed class PasswordHasherService : IPasswordService
{
    private readonly PasswordHasher<AuthAccount> _hasher = new();

    public string Hash(AuthAccount account, string password) => _hasher.HashPassword(account, password);

    public bool Verify(AuthAccount account, string password) =>
        _hasher.VerifyHashedPassword(account, account.PasswordHash, password) != PasswordVerificationResult.Failed;
}
