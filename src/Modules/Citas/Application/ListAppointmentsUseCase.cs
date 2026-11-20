using Ginecologia.Modules.Citas.Domain;

namespace Ginecologia.Modules.Citas.Application;

public sealed class ListAppointmentsUseCase(IAppointmentRepository appointments, AppointmentDtoMapper mapper)
{
    public async Task<PagedAppointmentResult<AppointmentDto>> ExecuteAsync(Guid? profesionalId, Guid? patientId, DateOnly? fecha, AppointmentStatus? estado, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = new AppointmentListQuery(profesionalId, patientId, fecha, estado, Math.Max(page, 1), Math.Clamp(pageSize <= 0 ? 10 : pageSize, 1, 100));
        var result = await appointments.ListAsync(query, cancellationToken);

        // Secuencial a proposito: cada MapAsync llama a GetPatientUseCase/GetStaffUseCase, que
        // usan los DbContext (Scoped) de Pacientes/Usuarios. Mapear en paralelo con Task.WhenAll
        // hace que varias llamadas compartan la misma instancia de DbContext al mismo tiempo,
        // lo que revienta con "A second operation was started on this context instance before a
        // previous operation completed" en cuanto hay 2+ citas en la pagina (reproducido real).
        var items = new List<AppointmentDto>(result.Items.Count);
        foreach (var item in result.Items)
            items.Add(await mapper.MapAsync(item, cancellationToken));

        return new PagedAppointmentResult<AppointmentDto>(items, result.TotalCount, result.Page, result.PageSize);
    }
}
