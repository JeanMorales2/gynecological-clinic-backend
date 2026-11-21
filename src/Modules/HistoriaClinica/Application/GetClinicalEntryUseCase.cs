namespace Ginecologia.Modules.HistoriaClinica.Application;

public sealed class GetClinicalEntryUseCase(IClinicalEntryRepository entries)
{
    public async Task<ClinicalEntryDto?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entry = await entries.FindByIdAsync(id, cancellationToken);
        return entry is null ? null : ClinicalEntryDto.From(entry);
    }
}
