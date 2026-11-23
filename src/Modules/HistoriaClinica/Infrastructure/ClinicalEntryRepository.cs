using Ginecologia.Modules.HistoriaClinica.Application;
using Ginecologia.Modules.HistoriaClinica.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.HistoriaClinica.Infrastructure;

public sealed class ClinicalEntryRepository(HistoriaClinicaDbContext dbContext) : IClinicalEntryRepository
{
    public Task<ClinicalEntry?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.ClinicalEntries.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<PagedClinicalEntryResult<ClinicalEntry>> ListAsync(ClinicalEntryListQuery query, CancellationToken cancellationToken = default)
    {
        IQueryable<ClinicalEntry> entries = dbContext.ClinicalEntries.AsNoTracking().Where(x => x.PatientId == query.PatientId);
        if (query.Tipo is { } tipo) entries = entries.Where(x => x.Tipo == tipo);

        var totalCount = await entries.CountAsync(cancellationToken);
        var items = await entries
            .OrderByDescending(x => x.Fecha)
            .ThenByDescending(x => x.CreatedAtUtc)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);
        return new PagedClinicalEntryResult<ClinicalEntry>(items, totalCount, query.Page, query.PageSize);
    }

    public async Task<IReadOnlyList<ClinicalEntry>> ListAllByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default) =>
        await dbContext.ClinicalEntries.AsNoTracking().Where(x => x.PatientId == patientId)
            .OrderBy(x => x.Fecha).ThenBy(x => x.CreatedAtUtc).ToListAsync(cancellationToken);

    public Task AddAsync(ClinicalEntry entry, CancellationToken cancellationToken = default) =>
        dbContext.ClinicalEntries.AddAsync(entry, cancellationToken).AsTask();

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
