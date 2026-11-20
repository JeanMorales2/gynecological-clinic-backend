using Ginecologia.Modules.Pacientes.Application;
using Ginecologia.Modules.Pacientes.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.Pacientes.Infrastructure;

public sealed class PatientRepository(PacientesDbContext dbContext) : IPatientRepository
{
    public Task<Patient?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Patients.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<PagedResult<Patient>> ListAsync(PatientListQuery query, CancellationToken cancellationToken = default)
    {
        IQueryable<Patient> patients = dbContext.Patients.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query.Search))
            patients = patients.Where(x => EF.Functions.ILike(x.NombreCompleto, $"%{query.Search}%"));

        var totalCount = await patients.CountAsync(cancellationToken);
        var items = await patients
            .OrderBy(x => x.NombreCompleto)
            .ThenBy(x => x.CreatedAtUtc)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Patient>(items, totalCount, query.Page, query.PageSize);
    }

    public Task AddAsync(Patient patient, CancellationToken cancellationToken = default) =>
        dbContext.Patients.AddAsync(patient, cancellationToken).AsTask();

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
