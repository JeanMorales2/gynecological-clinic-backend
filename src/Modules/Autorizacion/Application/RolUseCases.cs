using Ginecologia.Modules.Autorizacion.Domain;

namespace Ginecologia.Modules.Autorizacion.Application;

public sealed class ListRolesUseCase(IRolRepository roles)
{
    public async Task<IReadOnlyList<RolDto>> ExecuteAsync(CancellationToken ct = default) =>
        (await roles.ListAsync(ct)).Select(RolDto.From).ToList();
}

public sealed class CreateRolUseCase(IRolRepository roles)
{
    public async Task<RolDto> ExecuteAsync(string nombre, CancellationToken ct = default)
    {
        var rol = Rol.Create(nombre);
        if (await roles.FindByNombreAsync(rol.Nombre, ct) is not null) throw new RolConflictException(rol.Nombre);
        await roles.AddAsync(rol, ct);
        await roles.SaveChangesAsync(ct);
        return RolDto.From(rol);
    }
}

public sealed class DeleteRolUseCase(IRolRepository roles)
{
    public async Task ExecuteAsync(string nombre, CancellationToken ct = default)
    {
        var rol = await roles.FindByNombreAsync(nombre, ct) ?? throw new RolNotFoundException(nombre);
        if (rol.EsProtegido) throw new RolProtegidoException(nombre);
        roles.Remove(rol);
        await roles.SaveChangesAsync(ct);
    }
}

/// <summary>Validador de rol consumido por otros modulos (Auth, Usuarios) para confirmar que un rol existe antes de asignarlo a una cuenta.</summary>
public sealed class RoleExistsUseCase(IRolRepository roles)
{
    public async Task<bool> ExecuteAsync(string nombre, CancellationToken ct = default) =>
        await roles.FindByNombreAsync(nombre, ct) is not null;
}
