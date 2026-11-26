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
        role = RequiredRole(role);

        return new AuthAccount(Guid.NewGuid(), normalizedEmail, passwordHash, role, true, DateTime.UtcNow);
    }

    public void ChangeRole(string role)
    {
        Role = RequiredRole(role);
    }

    // Que el rol exista de verdad (catalogo dinamico del modulo Autorizacion) se valida en la capa de
    // aplicacion (AuthAccountManager via RoleExistsUseCase) antes de llamar a Create/ChangeRole; el dominio
    // solo garantiza que no llegue vacio.
    private static string RequiredRole(string value)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized)) throw new ArgumentException("El rol es obligatorio.", nameof(value));
        return normalized;
    }

    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("El hash de contraseña es obligatorio.", nameof(passwordHash));
        PasswordHash = passwordHash;
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;

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
