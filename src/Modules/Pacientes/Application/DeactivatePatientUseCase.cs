namespace Ginecologia.Modules.Pacientes.Application;

public sealed class DeactivatePatientUseCase(IPatientRepository patients)
{
    public async Task<PatientDto> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var patient = await patients.FindByIdAsync(id, cancellationToken) ?? throw new PatientNotFoundException(id);
        if (patient.Activo)
        {
            patient.Deactivate();
            await patients.SaveChangesAsync(cancellationToken);
        }
        return PatientDto.From(patient);
    }
}
