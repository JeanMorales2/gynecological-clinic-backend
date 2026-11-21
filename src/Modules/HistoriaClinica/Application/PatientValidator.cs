using Ginecologia.Modules.Pacientes.Application;

namespace Ginecologia.Modules.HistoriaClinica.Application;

public sealed class ClinicalEntryPatientValidator(GetPatientUseCase getPatient)
{
    public async Task ValidateAsync(Guid patientId, CancellationToken cancellationToken)
    {
        if (patientId == Guid.Empty) throw new ArgumentException("El paciente es obligatorio.", nameof(patientId));
        if (await getPatient.ExecuteAsync(patientId, cancellationToken) is null)
            throw new ArgumentException("El paciente indicado no existe.", nameof(patientId));
    }
}
