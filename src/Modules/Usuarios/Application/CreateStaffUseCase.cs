using System.Transactions;
using Ginecologia.Modules.Auth.Application;
using Ginecologia.Modules.Usuarios.Domain;

namespace Ginecologia.Modules.Usuarios.Application;

public sealed class CreateStaffUseCase(IAuthAccountManager accounts, IStaffProfileRepository profiles)
{
    public async Task<StaffDto> ExecuteAsync(CreateStaffCommand command, CancellationToken cancellationToken = default)
    {
        if (!AuthRoleValues.IsValid(command.Role)) throw new ArgumentException("El rol no es válido.", nameof(command));

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var account = await accounts.CreateAsync(new CreateAuthAccountRequest(command.Email, command.Password, command.Role), cancellationToken);
        var profile = StaffProfile.Create(account.Id, command.NombreCompleto, command.Telefono, command.Role);
        await profiles.AddAsync(profile, cancellationToken);
        await profiles.SaveChangesAsync(cancellationToken);
        transaction.Complete();
        return StaffDto.From(profile);
    }
}
