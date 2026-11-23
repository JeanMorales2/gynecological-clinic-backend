using Ginecologia.Modules.Pacientes.Domain;

namespace Ginecologia.Modules.Pacientes.Application;

public sealed class AddFamilyMedicalHistoryUseCase(IPatientRepository patients, IFamilyMedicalHistoryRepository familyMedicalHistories)
{
    public async Task<FamilyMedicalHistoryDto> ExecuteAsync(Guid patientId, string condicion, string parentesco, CancellationToken cancellationToken = default)
    {
        if (await patients.FindByIdAsync(patientId, cancellationToken) is null) throw new PatientNotFoundException(patientId);
        var familyMedicalHistory = FamilyMedicalHistory.Create(patientId, condicion, parentesco);
        await familyMedicalHistories.AddAsync(familyMedicalHistory, cancellationToken);
        await familyMedicalHistories.SaveChangesAsync(cancellationToken);
        return FamilyMedicalHistoryDto.From(familyMedicalHistory);
    }
}
