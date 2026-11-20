using System.Transactions;
using Ginecologia.Modules.Auth.Application;

namespace Ginecologia.Modules.Usuarios.Application;

public sealed class DeactivateStaffUseCase(IAuthAccountManager accounts, IStaffProfileRepository profiles)
{
    public async Task<StaffDto> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var profile = await profiles.FindByIdAsync(id, cancellationToken) ?? throw new StaffNotFoundException(id);
        if (!profile.Activo) return StaffDto.From(profile);

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await accounts.DeactivateAsync(id, cancellationToken);
        profile.Deactivate();
        await profiles.SaveChangesAsync(cancellationToken);
        transaction.Complete();
        return StaffDto.From(profile);
    }
}
