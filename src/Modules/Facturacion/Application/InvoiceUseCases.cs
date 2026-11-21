using Ginecologia.Modules.Facturacion.Domain;
namespace Ginecologia.Modules.Facturacion.Application;
public sealed class CreateInvoiceUseCase(IInvoiceRepository invoices, InvoiceValidator validator)
{
    public async Task<InvoiceDto> ExecuteAsync(CreateInvoiceCommand command, CancellationToken ct = default)
    { if (command.Items.Count == 0) throw new ArgumentException("La factura debe contener al menos un ítem.", nameof(command.Items)); await validator.ValidateAsync(command.PatientId, command.AppointmentId, ct); var number = $"F-{(await invoices.CountAsync(ct) + 1):D5}"; var invoice = Invoice.Create(command.PatientId, command.AppointmentId, number, DateOnly.FromDateTime(DateTime.UtcNow), command.Items, command.CreadoPorId); await invoices.AddAsync(invoice, ct); await invoices.SaveChangesAsync(ct); return InvoiceDto.From(invoice); }
}
public sealed class MarkInvoicePaidUseCase(IInvoiceRepository invoices)
{ public async Task<InvoiceDto> ExecuteAsync(Guid id, string metodoPago, CancellationToken ct = default) { var invoice = await invoices.FindByIdAsync(id, ct) ?? throw new InvoiceNotFoundException(id); invoice.MarkPaid(metodoPago); await invoices.SaveChangesAsync(ct); return InvoiceDto.From(invoice); } }
public sealed class VoidInvoiceUseCase(IInvoiceRepository invoices)
{ public async Task<InvoiceDto> ExecuteAsync(Guid id, CancellationToken ct = default) { var invoice = await invoices.FindByIdAsync(id, ct) ?? throw new InvoiceNotFoundException(id); invoice.Void(); await invoices.SaveChangesAsync(ct); return InvoiceDto.From(invoice); } }
public sealed class GetInvoiceUseCase(IInvoiceRepository invoices)
{ public async Task<InvoiceDto?> ExecuteAsync(Guid id, CancellationToken ct = default) { var invoice = await invoices.FindByIdAsync(id, ct); return invoice is null ? null : InvoiceDto.From(invoice); } }
public sealed class ListInvoicesUseCase(IInvoiceRepository invoices)
{ public async Task<PagedInvoiceResult<InvoiceDto>> ExecuteAsync(Guid? patientId, InvoiceStatus? estado, int page, int pageSize, CancellationToken ct = default) { if (page < 1 || pageSize is < 1 or > 100) throw new ArgumentException("La paginación es inválida."); var result = await invoices.ListAsync(new(patientId, estado, page, pageSize), ct); return new(result.Items.Select(InvoiceDto.From).ToList(), result.TotalCount, result.Page, result.PageSize); } }
