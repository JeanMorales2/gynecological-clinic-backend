using Ginecologia.Modules.Auth.Application;
using Ginecologia.Modules.Auth.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.Auth.Infrastructure;

public sealed class AuthAccountRepository(AuthDbContext dbContext) : IAuthAccountRepository
{
    public Task<AuthAccount?> FindByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        dbContext.Accounts.SingleOrDefaultAsync(x => x.Email == AuthAccount.NormalizeEmail(email), cancellationToken);

    public Task<AuthAccount?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Accounts.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> HasAccountsAsync(CancellationToken cancellationToken = default) => dbContext.Accounts.AnyAsync(cancellationToken);
    public Task AddAsync(AuthAccount account, CancellationToken cancellationToken = default) => dbContext.Accounts.AddAsync(account, cancellationToken).AsTask();
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
