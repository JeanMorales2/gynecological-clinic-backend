using Ginecologia.Modules.Auth.Application;
using Ginecologia.Modules.Auth.Domain;
using Xunit;

namespace Ginecologia.Modules.Tests.Auth;

internal sealed class FakePasswordService : IPasswordService
{
    public string Hash(AuthAccount account, string password) => $"hash:{password}";
    public bool Verify(AuthAccount account, string password) => account.PasswordHash == Hash(account, password);
}

internal sealed class FakeAuthAccountRepository : IAuthAccountRepository
{
    private readonly Dictionary<Guid, AuthAccount> _accounts = [];
    public void Seed(AuthAccount account) => _accounts[account.Id] = account;
    public Task<AuthAccount?> FindByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        Task.FromResult(_accounts.Values.FirstOrDefault(a => a.Email == email));
    public Task<AuthAccount?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_accounts.GetValueOrDefault(id));
    public Task<bool> HasAccountsAsync(CancellationToken cancellationToken = default) => Task.FromResult(_accounts.Count > 0);
    public Task AddAsync(AuthAccount account, CancellationToken cancellationToken = default) { _accounts[account.Id] = account; return Task.CompletedTask; }
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}

public class ChangePasswordUseCaseTests
{
    private static (ChangePasswordUseCase UseCase, FakeAuthAccountRepository Repository, AuthAccount Account) CrearEscenario(string passwordActual = "ClaveActual1")
    {
        var passwords = new FakePasswordService();
        var repository = new FakeAuthAccountRepository();
        var account = AuthAccount.Create("doctora@clinica.com", "hash-inicial", AuthRoles.Ginecologa);
        account.SetPasswordHash(passwords.Hash(account, passwordActual));
        repository.Seed(account);
        return (new ChangePasswordUseCase(repository, passwords), repository, account);
    }

    [Fact]
    public async Task ExecuteAsync_ConContrasenaActualCorrecta_ActualizaElHash()
    {
        var (useCase, _, account) = CrearEscenario();

        await useCase.ExecuteAsync(account.Id, "ClaveActual1", "ClaveNueva1");

        Assert.Equal("hash:ClaveNueva1", account.PasswordHash);
    }

    [Fact]
    public async Task ExecuteAsync_ConContrasenaActualIncorrecta_Lanza()
    {
        var (useCase, _, account) = CrearEscenario();

        await Assert.ThrowsAsync<InvalidCurrentPasswordException>(() =>
            useCase.ExecuteAsync(account.Id, "ClaveIncorrecta", "ClaveNueva1"));
    }

    [Theory]
    [InlineData("corta")]
    [InlineData("")]
    public async Task ExecuteAsync_ConContrasenaNuevaMuyCorta_Lanza(string nuevaPassword)
    {
        var (useCase, _, account) = CrearEscenario();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.ExecuteAsync(account.Id, "ClaveActual1", nuevaPassword));
    }

    [Fact]
    public async Task ExecuteAsync_ConCuentaInexistente_Lanza()
    {
        var (useCase, _, _) = CrearEscenario();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.ExecuteAsync(Guid.NewGuid(), "ClaveActual1", "ClaveNueva1"));
    }
}
