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
            command.AntecedentesPatologicosPersonales,
            command.AntecedentesPatologicosFamiliares,
            command.AntecedentesQuirurgicos,
            command.Menarca,
            command.Ciclos,
            command.Ivsa,
            command.Fum,
            command.Mpf,
            command.Pap,
            command.Complicaciones,
            command.TipoSangre,
            command.Gestas,
            command.Partos,
            command.Cesareas,
            command.Abortos);
        await patients.AddAsync(patient, cancellationToken);
        await patients.SaveChangesAsync(cancellationToken);
        return PatientDto.From(patient);
    }
}
