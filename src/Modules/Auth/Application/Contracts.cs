using Ginecologia.Modules.Auth.Domain;

namespace Ginecologia.Modules.Auth.Application;

public interface IAuthAccountRepository
{
    Task<AuthAccount?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<AuthAccount?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> HasAccountsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(AuthAccount account, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IPasswordService
{
    string Hash(AuthAccount account, string password);
    bool Verify(AuthAccount account, string password);
}

public interface ITokenIssuer
{
    IssuedToken Issue(AuthAccount account);
}

public sealed record PublicAuthUser(Guid Id, string Email, string Role)
{
    public static PublicAuthUser From(AuthAccount account) => new(account.Id, account.Email, account.Role);
}

public sealed record IssuedToken(string AccessToken, DateTime ExpiresAtUtc);
public sealed record LoginResult(string AccessToken, DateTime ExpiresAtUtc, PublicAuthUser User);
