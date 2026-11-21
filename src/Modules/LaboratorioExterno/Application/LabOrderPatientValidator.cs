using Ginecologia.Modules.Pacientes.Application;

namespace Ginecologia.Modules.LaboratorioExterno.Application;

public sealed class LabOrderPatientValidator(GetPatientUseCase patients)
{
    public async Task ValidateAsync(Guid patientId, CancellationToken cancellationToken) { if (await patients.ExecuteAsync(patientId, cancellationToken) is null) throw new ArgumentException("El paciente no existe.", nameof(patientId)); }
}
