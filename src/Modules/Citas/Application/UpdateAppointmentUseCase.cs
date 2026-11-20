using Ginecologia.Modules.Citas.Domain;

namespace Ginecologia.Modules.Citas.Application;

public sealed class UpdateAppointmentUseCase(IAppointmentRepository appointments, AppointmentParticipantsValidator participants, AppointmentDtoMapper mapper)
{
    public async Task<AppointmentDto> ExecuteAsync(Guid id, UpdateAppointmentCommand command, CancellationToken cancellationToken = default)
    {
        var appointment = await appointments.FindByIdAsync(id, cancellationToken) ?? throw new AppointmentNotFoundException(id);
        if (appointment.Estado != AppointmentStatus.PendienteConsulta)
            throw new InvalidOperationException("Solo se pueden editar citas pendientes de consulta.");

        await participants.ValidateAsync(command.PatientId, command.ProfesionalId, cancellationToken);
        // Create a transient aggregate first so the same UTC and duration rules used by
        // creation are enforced before calculating the prospective end time.
        var rescheduled = Appointment.Create(command.PatientId, command.ProfesionalId, command.FechaHoraInicio, command.DuracionMinutos, command.Notas);
        if (await appointments.HasOverlapAsync(rescheduled.ProfesionalId, rescheduled.FechaHoraInicio, rescheduled.FechaHoraFin, id, cancellationToken))
            throw new ArgumentException("El profesional ya tiene una cita que se solapa con ese horario.");

        appointment.Update(command.PatientId, command.ProfesionalId, command.FechaHoraInicio, command.DuracionMinutos, command.Notas);
        await appointments.SaveChangesAsync(cancellationToken);
        return await mapper.MapAsync(appointment, cancellationToken);
    }
}
