namespace Ginecologia.Modules.Usuarios.Application;

public sealed class ListStaffUseCase(IStaffProfileRepository profiles)
{
    public async Task<IReadOnlyList<StaffDto>> ExecuteAsync(CancellationToken cancellationToken = default) =>
        (await profiles.ListAsync(cancellationToken)).Select(StaffDto.From).ToArray();
}
