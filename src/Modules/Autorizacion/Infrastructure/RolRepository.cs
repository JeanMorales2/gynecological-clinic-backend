using Ginecologia.Modules.Autorizacion.Application;
using Ginecologia.Modules.Autorizacion.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.Autorizacion.Infrastructure;

public sealed class RolRepository(AutorizacionDbContext dbContext) : IRolRepository
{
    public async Task<IReadOnlyList<Rol>> ListAsync(CancellationToken ct = default) =>
        await dbContext.Roles.AsNoTracking().OrderBy(rol => rol.Nombre).ToListAsync(ct);

    public Task<Rol?> FindByNombreAsync(string nombre, CancellationToken ct = default) =>
        dbContext.Roles.FirstOrDefaultAsync(rol => rol.Nombre == nombre, ct);

    public async Task AddAsync(Rol rol, CancellationToken ct = default) =>
        await dbContext.Roles.AddAsync(rol, ct);

    public void Remove(Rol rol) => dbContext.Roles.Remove(rol);

    public Task SaveChangesAsync(CancellationToken ct = default) => dbContext.SaveChangesAsync(ct);
}
