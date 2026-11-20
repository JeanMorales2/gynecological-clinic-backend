using Ginecologia.Modules.Citas.Domain;
using Ginecologia.Modules.Pacientes.Application;
using Ginecologia.Modules.Usuarios.Application;

namespace Ginecologia.Modules.Citas.Application;

public sealed class AppointmentParticipantsValidator(GetPatientUseCase getPatient, GetStaffUseCase getStaff)
{
    public async Task ValidateAsync(Guid patientId, Guid profesionalId, CancellationToken cancellationToken)
    {
        var patient = await getPatient.ExecuteAsync(patientId, cancellationToken);
        if (patient is null) throw new ArgumentException("El paciente indicado no existe.", nameof(patientId));

        var profesional = await getStaff.ExecuteAsync(profesionalId, cancellationToken);
        if (profesional is null || !profesional.Activo || profesional.Role != "Ginecologa")
            throw new ArgumentException("El profesional indicado no existe o no es una ginecóloga activa.", nameof(profesionalId));
    }
}

public sealed class AppointmentDtoMapper(GetPatientUseCase getPatient, GetStaffUseCase getStaff)
{
    public async Task<AppointmentDto> MapAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        var patient = await getPatient.ExecuteAsync(appointment.PatientId, cancellationToken);
        var profesional = await getStaff.ExecuteAsync(appointment.ProfesionalId, cancellationToken);
        return new AppointmentDto(
            appointment.Id,
            appointment.PatientId,
            patient?.NombreCompleto ?? "Paciente no disponible",
            appointment.ProfesionalId,
            profesional?.NombreCompleto ?? "Profesional no disponible",
            appointment.FechaHoraInicio,
            appointment.FechaHoraFin,
            appointment.DuracionMinutos,
            appointment.Estado,
            appointment.Notas,
            appointment.CreatedAtUtc);
    }
}
