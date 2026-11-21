using Ginecologia.Modules.Facturacion.Domain;
namespace Ginecologia.Modules.Facturacion.Application;
public interface IInvoiceRepository { Task<Invoice?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default); Task<PagedInvoiceResult<Invoice>> ListAsync(InvoiceListQuery query, CancellationToken cancellationToken = default); Task<int> CountAsync(CancellationToken cancellationToken = default); Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default); Task SaveChangesAsync(CancellationToken cancellationToken = default); }
public sealed record CreateInvoiceCommand(Guid PatientId, Guid? AppointmentId, IReadOnlyList<InvoiceItemInput> Items, Guid CreadoPorId);
public sealed record InvoiceListQuery(Guid? PatientId, InvoiceStatus? Estado, int Page, int PageSize);
public sealed record PagedInvoiceResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
public sealed record InvoiceItemDto(string Concepto, int Cantidad, decimal PrecioUnitario) { public static InvoiceItemDto From(InvoiceItem item) => new(item.Concepto, item.Cantidad, item.PrecioUnitario); }
public sealed record InvoiceDto(Guid Id, Guid PatientId, Guid? AppointmentId, string NumeroFactura, DateOnly Fecha, IReadOnlyList<InvoiceItemDto> Items, decimal MontoTotal, InvoiceStatus Estado, string? MetodoPago, DateOnly? FechaPago, Guid CreadoPorId)
{ public static InvoiceDto From(Invoice invoice) => new(invoice.Id, invoice.PatientId, invoice.AppointmentId, invoice.NumeroFactura, invoice.Fecha, invoice.Items.Select(InvoiceItemDto.From).ToList(), invoice.MontoTotal, invoice.Estado, invoice.MetodoPago, invoice.FechaPago, invoice.CreadoPorId); }
public sealed class InvoiceNotFoundException(Guid id) : KeyNotFoundException($"No existe la factura {id}.");
