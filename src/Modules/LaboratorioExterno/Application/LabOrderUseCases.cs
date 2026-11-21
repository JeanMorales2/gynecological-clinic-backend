using System.Transactions;
using Ginecologia.Modules.HistoriaClinica.Application;
using Ginecologia.Modules.HistoriaClinica.Domain;
using Ginecologia.Modules.LaboratorioExterno.Domain;

namespace Ginecologia.Modules.LaboratorioExterno.Application;

public sealed class CreateLabOrderUseCase(ILabOrderRepository orders, LabOrderPatientValidator patients)
{
    public async Task<LabOrderDto> ExecuteAsync(CreateLabOrderCommand command, CancellationToken cancellationToken = default)
    {
        await patients.ValidateAsync(command.PatientId, cancellationToken);
        var order = LabOrder.Create(command.PatientId, command.TipoEstudio, command.LaboratorioNombre, command.FechaSolicitud, command.Observaciones, command.SolicitadoPorId);
        await orders.AddAsync(order, cancellationToken); await orders.SaveChangesAsync(cancellationToken); return LabOrderDto.From(order);
    }
}

public sealed class MarkLabOrderReceivedUseCase(ILabOrderRepository orders, CreateClinicalEntryUseCase createClinicalEntry)
{
    public async Task<LabOrderDto> ExecuteAsync(Guid id, string resultadoResumen, string? observaciones, CancellationToken cancellationToken = default)
    {
        var order = await orders.FindByIdAsync(id, cancellationToken) ?? throw new LabOrderNotFoundException(id);
        order.MarkReceived(resultadoResumen, observaciones);
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await orders.SaveChangesAsync(cancellationToken);
        await createClinicalEntry.ExecuteAsync(new CreateClinicalEntryCommand(order.PatientId, ClinicalEntryType.Laboratorio, order.FechaResultado!.Value, order.TipoEstudio, order.ResultadoResumen!, null, order.Observaciones, order.SolicitadoPorId), cancellationToken);
        transaction.Complete();
        return LabOrderDto.From(order);
    }
}

public sealed class CancelLabOrderUseCase(ILabOrderRepository orders)
{
    public async Task<LabOrderDto> ExecuteAsync(Guid id, CancellationToken cancellationToken = default) { var order = await orders.FindByIdAsync(id, cancellationToken) ?? throw new LabOrderNotFoundException(id); order.Cancel(); await orders.SaveChangesAsync(cancellationToken); return LabOrderDto.From(order); }
}
public sealed class GetLabOrderUseCase(ILabOrderRepository orders)
{ public async Task<LabOrderDto?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default) { var order = await orders.FindByIdAsync(id, cancellationToken); return order is null ? null : LabOrderDto.From(order); } }
public sealed class ListLabOrdersUseCase(ILabOrderRepository orders, LabOrderPatientValidator patients)
{
    public async Task<PagedLabOrderResult<LabOrderDto>> ExecuteAsync(Guid patientId, LabOrderStatus? estado, int page, int pageSize, CancellationToken cancellationToken = default)
    { if (page < 1 || pageSize is < 1 or > 100) throw new ArgumentException("La paginación es inválida."); await patients.ValidateAsync(patientId, cancellationToken); var result = await orders.ListAsync(new LabOrderListQuery(patientId, estado, page, pageSize), cancellationToken); return new(result.Items.Select(LabOrderDto.From).ToList(), result.TotalCount, result.Page, result.PageSize); }
}
