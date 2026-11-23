using Ginecologia.Modules.LaboratorioExterno.Domain;

namespace Ginecologia.Modules.LaboratorioExterno.Application;

public interface ILabOrderRepository
{
    Task<LabOrder?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedLabOrderResult<LabOrder>> ListAsync(LabOrderListQuery query, CancellationToken cancellationToken = default);
    Task AddAsync(LabOrder order, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface ILabOrderAttachmentRepository
{
    Task<LabOrderAttachment?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LabOrderAttachmentDto>> ListByLabOrderIdAsync(Guid labOrderId, CancellationToken cancellationToken = default);
    Task AddAsync(LabOrderAttachment attachment, CancellationToken cancellationToken = default);
    Task RemoveAsync(LabOrderAttachment attachment, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public sealed record CreateLabOrderCommand(Guid PatientId, string TipoEstudio, string LaboratorioNombre, DateOnly FechaSolicitud, string? Observaciones, Guid SolicitadoPorId);
public sealed record LabOrderListQuery(Guid PatientId, LabOrderStatus? Estado, int Page, int PageSize);
public sealed record PagedLabOrderResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
public sealed record LabOrderDto(Guid Id, Guid PatientId, string TipoEstudio, string LaboratorioNombre, DateOnly FechaSolicitud, LabOrderStatus Estado, string? ResultadoResumen, DateOnly? FechaResultado, string? Observaciones, Guid SolicitadoPorId)
{ public static LabOrderDto From(LabOrder order) => new(order.Id, order.PatientId, order.TipoEstudio, order.LaboratorioNombre, order.FechaSolicitud, order.Estado, order.ResultadoResumen, order.FechaResultado, order.Observaciones, order.SolicitadoPorId); }
public sealed record LabOrderAttachmentDto(Guid Id, Guid LabOrderId, string NombreArchivo, string ContentType, int TamanoBytes, Guid SubidoPorId, DateTime CreatedAtUtc)
{ public static LabOrderAttachmentDto From(LabOrderAttachment attachment) => new(attachment.Id, attachment.LabOrderId, attachment.NombreArchivo, attachment.ContentType, attachment.TamanoBytes, attachment.SubidoPorId, attachment.CreatedAtUtc); }
public sealed record LabOrderAttachmentContent(byte[] Contenido, string ContentType, string NombreArchivo);
public sealed class LabOrderNotFoundException(Guid id) : KeyNotFoundException($"No existe la orden de laboratorio {id}.");
