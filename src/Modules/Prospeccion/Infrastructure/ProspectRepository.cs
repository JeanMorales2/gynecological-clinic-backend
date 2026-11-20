using Ginecologia.Modules.Prospeccion.Application;
using Ginecologia.Modules.Prospeccion.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.Prospeccion.Infrastructure;

public sealed class ProspectRepository(ProspeccionDbContext dbContext) : IProspectRepository
{
    public Task<Prospect?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Prospects.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<PagedProspectResult<Prospect>> ListAsync(ProspectListQuery query, CancellationToken cancellationToken = default)
    {
        IQueryable<Prospect> prospects = dbContext.Prospects.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query.Search))
            prospects = prospects.Where(x => EF.Functions.ILike(x.NombreCompleto, $"%{query.Search}%"));
        if (query.Estado is not null)
            prospects = prospects.Where(x => x.Estado == query.Estado);

        var totalCount = await prospects.CountAsync(cancellationToken);
        var items = await prospects
            .OrderBy(x => x.NombreCompleto)
            .ThenBy(x => x.CreatedAtUtc)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedProspectResult<Prospect>(items, totalCount, query.Page, query.PageSize);
    }

    public Task AddAsync(Prospect prospect, CancellationToken cancellationToken = default) =>
        dbContext.Prospects.AddAsync(prospect, cancellationToken).AsTask();

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
