using Ginecologia.Modules.HistoriaClinica.Domain;

namespace Ginecologia.Modules.HistoriaClinica.Application;

public sealed class CreateClinicalEntryUseCase(IClinicalEntryRepository entries, ClinicalEntryPatientValidator patients)
{
    public async Task<ClinicalEntryDto> ExecuteAsync(CreateClinicalEntryCommand command, CancellationToken cancellationToken = default)
    {
        await patients.ValidateAsync(command.PatientId, cancellationToken);
        var entry = ClinicalEntry.Create(command.PatientId, command.Tipo, command.Fecha, command.Motivo, command.Hallazgos, command.Diagnostico, command.Indicaciones, command.Tratamiento, command.Medicamentos, command.Examenes, command.PesoKg, command.AlturaCm, command.RegistradoPorId);
        await entries.AddAsync(entry, cancellationToken);
        await entries.SaveChangesAsync(cancellationToken);
        return ClinicalEntryDto.From(entry);
    }
}
