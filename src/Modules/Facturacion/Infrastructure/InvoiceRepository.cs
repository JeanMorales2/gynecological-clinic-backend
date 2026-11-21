using Ginecologia.Modules.Facturacion.Application;
using Ginecologia.Modules.Facturacion.Domain;
using Microsoft.EntityFrameworkCore;
namespace Ginecologia.Modules.Facturacion.Infrastructure;
public sealed class InvoiceRepository(FacturacionDbContext db) : IInvoiceRepository
{
    public Task<Invoice?> FindByIdAsync(Guid id, CancellationToken ct = default) => db.Invoices.Include(x => x.Items.OrderBy(i => i.Orden)).SingleOrDefaultAsync(x => x.Id == id, ct);
    public async Task<PagedInvoiceResult<Invoice>> ListAsync(InvoiceListQuery query, CancellationToken ct = default) { IQueryable<Invoice> invoices = db.Invoices.AsNoTracking().Include(x => x.Items.OrderBy(i => i.Orden)); if (query.PatientId is { } patientId) invoices = invoices.Where(x => x.PatientId == patientId); if (query.Estado is { } estado) invoices = invoices.Where(x => x.Estado == estado); var total = await invoices.CountAsync(ct); var items = await invoices.OrderByDescending(x => x.Fecha).ThenByDescending(x => x.NumeroFactura).Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct); return new(items, total, query.Page, query.PageSize); }
    public Task<int> CountAsync(CancellationToken ct = default) => db.Invoices.CountAsync(ct); public Task AddAsync(Invoice invoice, CancellationToken ct = default) => db.Invoices.AddAsync(invoice, ct).AsTask(); public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
