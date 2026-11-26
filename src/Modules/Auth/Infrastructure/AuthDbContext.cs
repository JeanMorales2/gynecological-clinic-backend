using Ginecologia.Modules.Auth.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.Auth.Infrastructure;

public sealed class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<AuthAccount> Accounts => Set<AuthAccount>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("auth");
        var account = modelBuilder.Entity<AuthAccount>();
        account.ToTable("accounts");
        account.HasKey(x => x.Id);
        account.Property(x => x.Id).HasColumnName("id");
        account.Property(x => x.Email).HasColumnName("email").HasMaxLength(320).IsRequired();
        account.HasIndex(x => x.Email).IsUnique();
        account.Property(x => x.PasswordHash).HasColumnName("password_hash").IsRequired();
        account.Property(x => x.Role).HasColumnName("role").HasMaxLength(32).IsRequired();
        account.Property(x => x.IsActive).HasColumnName("is_active").IsRequired();
        account.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();

        var resetToken = modelBuilder.Entity<PasswordResetToken>();
        resetToken.ToTable("password_reset_tokens");
        resetToken.HasKey(x => x.Id);
        resetToken.Property(x => x.Id).HasColumnName("id");
        resetToken.Property(x => x.AccountId).HasColumnName("account_id").IsRequired();
        resetToken.Property(x => x.TokenHash).HasColumnName("token_hash").HasMaxLength(100).IsRequired();
        resetToken.Property(x => x.ExpiresAtUtc).HasColumnName("expires_at_utc").IsRequired();
        resetToken.Property(x => x.UsedAtUtc).HasColumnName("used_at_utc");
        resetToken.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        resetToken.HasIndex(x => x.TokenHash).IsUnique();
        resetToken.HasOne<AuthAccount>().WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Cascade);
    }
}
