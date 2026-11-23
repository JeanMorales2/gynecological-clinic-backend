namespace Ginecologia.Modules.HistoriaClinica.Application;

public sealed class UpdateClinicalEntryUseCase(IClinicalEntryRepository entries, ClinicalEntryPatientValidator patients)
{
    public async Task<ClinicalEntryDto> ExecuteAsync(Guid id, UpdateClinicalEntryCommand command, CancellationToken cancellationToken = default)
    {
        var entry = await entries.FindByIdAsync(id, cancellationToken) ?? throw new ClinicalEntryNotFoundException(id);
        await patients.ValidateAsync(entry.PatientId, cancellationToken);
        entry.Update(command.Tipo, command.Fecha, command.Motivo, command.Hallazgos, command.Diagnostico, command.Indicaciones, command.Tratamiento, command.Medicamentos, command.Examenes, command.PesoKg, command.AlturaCm);
        await entries.SaveChangesAsync(cancellationToken);
        return ClinicalEntryDto.From(entry);
    }
}
