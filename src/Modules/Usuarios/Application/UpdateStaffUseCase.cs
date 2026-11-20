using System.Transactions;
using Ginecologia.Modules.Auth.Application;

namespace Ginecologia.Modules.Usuarios.Application;

public sealed class UpdateStaffUseCase(IAuthAccountManager accounts, IStaffProfileRepository profiles)
{
    public async Task<StaffDto> ExecuteAsync(Guid id, UpdateStaffCommand command, CancellationToken cancellationToken = default)
    {
        if (!AuthRoleValues.IsValid(command.Role)) throw new ArgumentException("El rol no es válido.", nameof(command));
        var profile = await profiles.FindByIdAsync(id, cancellationToken) ?? throw new StaffNotFoundException(id);

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await accounts.ChangeRoleAsync(id, command.Role, cancellationToken);
        profile.Update(command.NombreCompleto, command.Telefono, command.Role);
        await profiles.SaveChangesAsync(cancellationToken);
        transaction.Complete();
        return StaffDto.From(profile);
    }
}
