namespace Ginecologia.Modules.Usuarios.Application;

public sealed class GetStaffUseCase(IStaffProfileRepository profiles)
{
    public async Task<StaffDto?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var profile = await profiles.FindByIdAsync(id, cancellationToken);
        return profile is null ? null : StaffDto.From(profile);
    }
}
