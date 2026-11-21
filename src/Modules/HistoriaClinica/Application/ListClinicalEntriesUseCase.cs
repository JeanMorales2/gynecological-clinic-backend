using Ginecologia.Modules.HistoriaClinica.Domain;

namespace Ginecologia.Modules.HistoriaClinica.Application;

public sealed class ListClinicalEntriesUseCase(IClinicalEntryRepository entries, ClinicalEntryPatientValidator patients)
{
    public async Task<PagedClinicalEntryResult<ClinicalEntryDto>> ExecuteAsync(Guid patientId, ClinicalEntryType? tipo, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        await patients.ValidateAsync(patientId, cancellationToken);
        var query = new ClinicalEntryListQuery(patientId, tipo, Math.Max(page, 1), Math.Clamp(pageSize <= 0 ? 10 : pageSize, 1, 100));
        var result = await entries.ListAsync(query, cancellationToken);

        // Mapping is intentionally sequential. If this DTO later resolves data from another
        // module, keep its scoped DbContext calls one at a time.
        var items = new List<ClinicalEntryDto>(result.Items.Count);
        foreach (var entry in result.Items)
            items.Add(ClinicalEntryDto.From(entry));

        return new PagedClinicalEntryResult<ClinicalEntryDto>(items, result.TotalCount, result.Page, result.PageSize);
    }
}
