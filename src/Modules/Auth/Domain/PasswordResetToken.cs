namespace Ginecologia.Modules.Auth.Domain;

public sealed class PasswordResetToken
{
    private PasswordResetToken() { }

    private PasswordResetToken(Guid id, Guid accountId, string tokenHash, DateTime expiresAtUtc)
    {
        Id = id;
        AccountId = accountId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime? UsedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static PasswordResetToken Create(Guid accountId, string tokenHash, TimeSpan validFor) =>
        new(Guid.NewGuid(), accountId, tokenHash, DateTime.UtcNow.Add(validFor));

    public bool IsValid(DateTime nowUtc) => UsedAtUtc is null && ExpiresAtUtc > nowUtc;

    public void MarkUsed() => UsedAtUtc = DateTime.UtcNow;
}
