namespace Ginecologia.Modules.Pacientes.Application;

public sealed class UpdatePatientUseCase(IPatientRepository patients)
{
    public async Task<PatientDto> ExecuteAsync(Guid id, UpdatePatientCommand command, CancellationToken cancellationToken = default)
    {
        var patient = await patients.FindByIdAsync(id, cancellationToken) ?? throw new PatientNotFoundException(id);
        patient.Update(
            command.NombreCompleto,
            command.FechaNacimiento,
            command.Telefono,
            command.Email,
            command.DocumentoIdentidad,
            command.Alergias,
            command.AntecedentesRelevantes,
            command.EnfermedadesCronicas,
            command.EtsIts,
            command.Vicios);
        await patients.SaveChangesAsync(cancellationToken);
        return PatientDto.From(patient);
    }
}
