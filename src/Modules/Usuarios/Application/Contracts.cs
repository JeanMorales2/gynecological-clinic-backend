using Ginecologia.Modules.Usuarios.Domain;

namespace Ginecologia.Modules.Usuarios.Application;

public interface IStaffProfileRepository
{
    Task<StaffProfile?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StaffProfile>> ListAsync(CancellationToken cancellationToken = default);
    Task AddAsync(StaffProfile profile, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public sealed record StaffDto(Guid Id, string NombreCompleto, string? Telefono, string Role, bool Activo, DateTime CreatedAtUtc)
{
    public static StaffDto From(StaffProfile profile) => new(profile.Id, profile.NombreCompleto, profile.Telefono, profile.Role, profile.Activo, profile.CreatedAtUtc);
}

public sealed record CreateStaffCommand(string Email, string Password, string NombreCompleto, string? Telefono, string Role);
public sealed record UpdateStaffCommand(string NombreCompleto, string? Telefono, string Role);
public sealed class StaffNotFoundException : KeyNotFoundException
{
    public StaffNotFoundException(Guid id) : base($"No existe el usuario {id}.") { }
}
