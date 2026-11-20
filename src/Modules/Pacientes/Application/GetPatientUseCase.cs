namespace Ginecologia.Modules.Pacientes.Application;

public sealed class GetPatientUseCase(IPatientRepository patients)
{
    public async Task<PatientDto?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var patient = await patients.FindByIdAsync(id, cancellationToken);
        return patient is null ? null : PatientDto.From(patient);
    }
}
