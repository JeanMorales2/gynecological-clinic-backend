using Ginecologia.Modules.Usuarios.Application;
using Ginecologia.Modules.Usuarios.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.Usuarios.Infrastructure;

public sealed class StaffProfileRepository(UsuariosDbContext dbContext) : IStaffProfileRepository
{
    public Task<StaffProfile?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Perfiles.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<StaffProfile>> ListAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Perfiles.OrderBy(x => x.NombreCompleto).ToListAsync(cancellationToken);

    public Task AddAsync(StaffProfile profile, CancellationToken cancellationToken = default) =>
        dbContext.Perfiles.AddAsync(profile, cancellationToken).AsTask();

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
