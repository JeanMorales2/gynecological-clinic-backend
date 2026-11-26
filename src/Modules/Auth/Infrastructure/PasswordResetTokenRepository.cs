using Ginecologia.Modules.Auth.Application;
using Ginecologia.Modules.Auth.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.Auth.Infrastructure;

public sealed class PasswordResetTokenRepository(AuthDbContext dbContext) : IPasswordResetTokenRepository
{
    public Task AddAsync(PasswordResetToken token, CancellationToken cancellationToken = default) =>
        dbContext.PasswordResetTokens.AddAsync(token, cancellationToken).AsTask();

    public Task<PasswordResetToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        dbContext.PasswordResetTokens.SingleOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
