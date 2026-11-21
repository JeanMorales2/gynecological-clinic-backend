using Ginecologia.Modules.LaboratorioExterno.Domain;

namespace Ginecologia.Modules.LaboratorioExterno.Application;

public interface ILabOrderRepository
{
    Task<LabOrder?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedLabOrderResult<LabOrder>> ListAsync(LabOrderListQuery query, CancellationToken cancellationToken = default);
    Task AddAsync(LabOrder order, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public sealed record CreateLabOrderCommand(Guid PatientId, string TipoEstudio, string LaboratorioNombre, DateOnly FechaSolicitud, string? Observaciones, Guid SolicitadoPorId);
public sealed record LabOrderListQuery(Guid PatientId, LabOrderStatus? Estado, int Page, int PageSize);
public sealed record PagedLabOrderResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
public sealed record LabOrderDto(Guid Id, Guid PatientId, string TipoEstudio, string LaboratorioNombre, DateOnly FechaSolicitud, LabOrderStatus Estado, string? ResultadoResumen, DateOnly? FechaResultado, string? Observaciones, Guid SolicitadoPorId)
{ public static LabOrderDto From(LabOrder order) => new(order.Id, order.PatientId, order.TipoEstudio, order.LaboratorioNombre, order.FechaSolicitud, order.Estado, order.ResultadoResumen, order.FechaResultado, order.Observaciones, order.SolicitadoPorId); }
public sealed class LabOrderNotFoundException(Guid id) : KeyNotFoundException($"No existe la orden de laboratorio {id}.");
