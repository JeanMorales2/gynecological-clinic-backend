namespace Ginecologia.Modules.Citas.Application;

public sealed class ChangeAppointmentStatusUseCase(IAppointmentRepository appointments, AppointmentDtoMapper mapper)
{
    public async Task<AppointmentDto> ExecuteAsync(Guid id, Domain.AppointmentStatus estado, CancellationToken cancellationToken = default)
    {
        var appointment = await appointments.FindByIdAsync(id, cancellationToken) ?? throw new AppointmentNotFoundException(id);
        appointment.ChangeStatus(estado);
        await appointments.SaveChangesAsync(cancellationToken);
        return await mapper.MapAsync(appointment, cancellationToken);
    }
}
