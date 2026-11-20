namespace Ginecologia.Modules.Citas.Application;

public sealed class GetAppointmentUseCase(IAppointmentRepository appointments, AppointmentDtoMapper mapper)
{
    public async Task<AppointmentDto?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appointment = await appointments.FindByIdAsync(id, cancellationToken);
        return appointment is null ? null : await mapper.MapAsync(appointment, cancellationToken);
    }
}
