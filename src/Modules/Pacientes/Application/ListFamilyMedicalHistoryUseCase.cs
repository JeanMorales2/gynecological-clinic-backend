namespace Ginecologia.Modules.Pacientes.Application;

public sealed class ListFamilyMedicalHistoryUseCase(IFamilyMedicalHistoryRepository familyMedicalHistories)
{
    public async Task<IReadOnlyList<FamilyMedicalHistoryDto>> ExecuteAsync(Guid patientId, CancellationToken cancellationToken = default) =>
        (await familyMedicalHistories.ListByPatientIdAsync(patientId, cancellationToken)).Select(FamilyMedicalHistoryDto.From).ToList();
}
