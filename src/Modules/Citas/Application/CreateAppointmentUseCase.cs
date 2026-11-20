using Ginecologia.Modules.Citas.Domain;

namespace Ginecologia.Modules.Citas.Application;

public sealed class CreateAppointmentUseCase(IAppointmentRepository appointments, AppointmentParticipantsValidator participants, AppointmentDtoMapper mapper)
{
    public async Task<AppointmentDto> ExecuteAsync(CreateAppointmentCommand command, CancellationToken cancellationToken = default)
    {
        await participants.ValidateAsync(command.PatientId, command.ProfesionalId, cancellationToken);
        var appointment = Appointment.Create(command.PatientId, command.ProfesionalId, command.FechaHoraInicio, command.DuracionMinutos, command.Notas);
        if (await appointments.HasOverlapAsync(appointment.ProfesionalId, appointment.FechaHoraInicio, appointment.FechaHoraFin, cancellationToken: cancellationToken))
            throw new ArgumentException("El profesional ya tiene una cita que se solapa con ese horario.");

        await appointments.AddAsync(appointment, cancellationToken);
        await appointments.SaveChangesAsync(cancellationToken);
        return await mapper.MapAsync(appointment, cancellationToken);
    }
}
