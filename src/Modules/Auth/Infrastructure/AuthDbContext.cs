using Ginecologia.Modules.Auth.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.Auth.Infrastructure;

public sealed class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<AuthAccount> Accounts => Set<AuthAccount>();

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
        account.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
    }
}
