namespace Ginecologia.Modules.Auth.Domain;

public sealed class AuthAccount
{
    private AuthAccount() { }

    private AuthAccount(Guid id, string email, string passwordHash, string role, bool isActive, DateTime createdAtUtc)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static AuthAccount Create(string email, string passwordHash, string role)
    {
        var normalizedEmail = NormalizeEmail(email);
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("El hash de contraseña es obligatorio.", nameof(passwordHash));
        if (role is not AuthRoles.Admin and not AuthRoles.Recepcionista and not AuthRoles.Ginecologa)
            throw new ArgumentException("El rol no es válido.", nameof(role));

        return new AuthAccount(Guid.NewGuid(), normalizedEmail, passwordHash, role, true, DateTime.UtcNow);
    }

    public void ChangeRole(string role)
    {
        if (role is not AuthRoles.Admin and not AuthRoles.Recepcionista and not AuthRoles.Ginecologa)
            throw new ArgumentException("El rol no es válido.", nameof(role));

        Role = role;
    }

    public void Deactivate() => IsActive = false;

    public static string NormalizeEmail(string email)
    {
        var normalized = email.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalized) || !normalized.Contains('@'))
            throw new ArgumentException("El correo electrónico no es válido.", nameof(email));

        return normalized;
    }
}

public static class AuthRoles
{
    public const string Admin = "Admin";
    public const string Recepcionista = "Recepcionista";
    public const string Ginecologa = "Ginecologa";
}
