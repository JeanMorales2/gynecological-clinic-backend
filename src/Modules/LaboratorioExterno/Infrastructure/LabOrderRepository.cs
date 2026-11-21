using Ginecologia.Modules.LaboratorioExterno.Application;
using Ginecologia.Modules.LaboratorioExterno.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.LaboratorioExterno.Infrastructure;
public sealed class LabOrderRepository(LaboratorioExternoDbContext db) : ILabOrderRepository
{
    public Task<LabOrder?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) => db.LabOrders.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    public async Task<PagedLabOrderResult<LabOrder>> ListAsync(LabOrderListQuery query, CancellationToken cancellationToken = default) { IQueryable<LabOrder> orders = db.LabOrders.AsNoTracking().Where(x => x.PatientId == query.PatientId); if (query.Estado is { } estado) orders = orders.Where(x => x.Estado == estado); var total = await orders.CountAsync(cancellationToken); var items = await orders.OrderByDescending(x => x.FechaSolicitud).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(cancellationToken); return new(items, total, query.Page, query.PageSize); }
    public Task AddAsync(LabOrder order, CancellationToken cancellationToken = default) => db.LabOrders.AddAsync(order, cancellationToken).AsTask();
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);
}
