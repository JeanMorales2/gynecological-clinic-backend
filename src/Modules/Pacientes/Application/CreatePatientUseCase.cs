using Ginecologia.Modules.Pacientes.Domain;

namespace Ginecologia.Modules.Pacientes.Application;

public sealed class CreatePatientUseCase(IPatientRepository patients)
{
    public async Task<PatientDto> ExecuteAsync(CreatePatientCommand command, CancellationToken cancellationToken = default)
    {
        var patient = Patient.Create(
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
        await patients.AddAsync(patient, cancellationToken);
        await patients.SaveChangesAsync(cancellationToken);
        return PatientDto.From(patient);
    }
}
