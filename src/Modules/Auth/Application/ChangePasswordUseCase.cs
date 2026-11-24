namespace Ginecologia.Modules.Auth.Application;

public sealed class ChangePasswordUseCase(IAuthAccountRepository accounts, IPasswordService passwords)
{
    private const int MinPasswordLength = 8;

    public async Task ExecuteAsync(Guid accountId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var account = await accounts.FindByIdAsync(accountId, cancellationToken)
            ?? throw new KeyNotFoundException("No existe la cuenta de autenticación.");

        if (string.IsNullOrWhiteSpace(currentPassword) || !passwords.Verify(account, currentPassword))
            throw new InvalidCurrentPasswordException();

        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < MinPasswordLength)
            throw new ArgumentException($"La nueva contraseña debe tener al menos {MinPasswordLength} caracteres.", nameof(newPassword));

        account.SetPasswordHash(passwords.Hash(account, newPassword));
        await accounts.SaveChangesAsync(cancellationToken);
    }
}
