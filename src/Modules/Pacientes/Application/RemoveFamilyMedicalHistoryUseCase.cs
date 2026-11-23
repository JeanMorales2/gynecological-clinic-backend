namespace Ginecologia.Modules.Pacientes.Application;

public sealed class RemoveFamilyMedicalHistoryUseCase(IFamilyMedicalHistoryRepository familyMedicalHistories)
{
    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var familyMedicalHistory = await familyMedicalHistories.FindByIdAsync(id, cancellationToken) ?? throw new FamilyMedicalHistoryNotFoundException(id);
        familyMedicalHistories.RemoveAsync(familyMedicalHistory);
        await familyMedicalHistories.SaveChangesAsync(cancellationToken);
    }
}
