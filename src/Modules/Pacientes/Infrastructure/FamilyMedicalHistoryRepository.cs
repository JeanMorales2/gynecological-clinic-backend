using Ginecologia.Modules.Pacientes.Application;
using Ginecologia.Modules.Pacientes.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.Pacientes.Infrastructure;

public sealed class FamilyMedicalHistoryRepository(PacientesDbContext dbContext) : IFamilyMedicalHistoryRepository
{
    public async Task<IReadOnlyList<FamilyMedicalHistory>> ListByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default) =>
        await dbContext.FamilyMedicalHistories.AsNoTracking()
            .Where(x => x.PatientId == patientId)
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public Task<FamilyMedicalHistory?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.FamilyMedicalHistories.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task AddAsync(FamilyMedicalHistory familyMedicalHistory, CancellationToken cancellationToken = default) =>
        dbContext.FamilyMedicalHistories.AddAsync(familyMedicalHistory, cancellationToken).AsTask();

    public void RemoveAsync(FamilyMedicalHistory familyMedicalHistory) => dbContext.FamilyMedicalHistories.Remove(familyMedicalHistory);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
