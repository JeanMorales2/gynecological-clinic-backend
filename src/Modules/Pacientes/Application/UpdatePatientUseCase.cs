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
        await patients.SaveChangesAsync(cancellationToken);
        return PatientDto.From(patient);
    }
}
