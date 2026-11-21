namespace Ginecologia.Modules.Pacientes.Application;
public sealed class ListActivePatientsBirthdaysUseCase(IPatientRepository patients)
{ public async Task<IReadOnlyList<PatientBirthdayDto>> ExecuteAsync(CancellationToken ct = default) => (await patients.ListActiveAsync(ct)).Select(x => new PatientBirthdayDto(x.Id, x.NombreCompleto, x.FechaNacimiento.Month, x.FechaNacimiento.Day)).ToList(); }
