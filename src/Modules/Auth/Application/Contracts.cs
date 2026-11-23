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

/// <summary>Public in-process contract consumed by the Usuarios module; it deliberately exposes no Auth domain type.</summary>
public interface IAuthAccountManager
{
    Task<AuthAccountSummary> CreateAsync(CreateAuthAccountRequest request, CancellationToken cancellationToken = default);
    Task ChangeRoleAsync(Guid accountId, string role, CancellationToken cancellationToken = default);
    Task DeactivateAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task ActivateAsync(Guid accountId, CancellationToken cancellationToken = default);
}

public sealed record CreateAuthAccountRequest(string Email, string Password, string Role);
public sealed record AuthAccountSummary(Guid Id, string Email, string Role);

public static class AuthRoleValues
{
    public const string Admin = "Admin";
    public const string Recepcionista = "Recepcionista";
    public const string Ginecologa = "Ginecologa";

    public static bool IsValid(string role) => role is Admin or Recepcionista or Ginecologa;
}

public sealed class AuthAccountConflictException(string email)
    : InvalidOperationException($"Ya existe una cuenta para {email}.");

public sealed record PublicAuthUser(Guid Id, string Email, string Role)
{
    public static PublicAuthUser From(AuthAccount account) => new(account.Id, account.Email, account.Role);
}

public sealed record IssuedToken(string AccessToken, DateTime ExpiresAtUtc);
public sealed record LoginResult(string AccessToken, DateTime ExpiresAtUtc, PublicAuthUser User);
