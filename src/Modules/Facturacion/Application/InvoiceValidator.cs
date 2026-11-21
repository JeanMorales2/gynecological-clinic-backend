using Ginecologia.Modules.Citas.Application;
using Ginecologia.Modules.Pacientes.Application;
namespace Ginecologia.Modules.Facturacion.Application;
public sealed class InvoiceValidator(GetPatientUseCase patients, GetAppointmentUseCase appointments)
{
    public async Task ValidateAsync(Guid patientId, Guid? appointmentId, CancellationToken ct)
    { if (await patients.ExecuteAsync(patientId, ct) is null) throw new ArgumentException("El paciente no existe.", nameof(patientId)); if (appointmentId is { } id) { var appointment = await appointments.ExecuteAsync(id, ct) ?? throw new ArgumentException("La cita no existe.", nameof(appointmentId)); if (appointment.PatientId != patientId) throw new ArgumentException("La cita no pertenece al paciente de la factura.", nameof(appointmentId)); } }
}
