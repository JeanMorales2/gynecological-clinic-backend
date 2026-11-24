using Ginecologia.Modules.Pacientes.Domain;

namespace Ginecologia.Modules.Pacientes.Application;

public interface IPatientRepository
{
    Task<Patient?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Patient>> ListAsync(PatientListQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Patient>> ListActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Patient patient, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IFamilyMedicalHistoryRepository
{
    Task<IReadOnlyList<FamilyMedicalHistory>> ListByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<FamilyMedicalHistory?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(FamilyMedicalHistory familyMedicalHistory, CancellationToken cancellationToken = default);
    void RemoveAsync(FamilyMedicalHistory familyMedicalHistory);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
public sealed record PatientBirthdayDto(Guid PatientId, string NombreCompleto, string? Email, int MesNacimiento, int DiaNacimiento);

public sealed record PatientDto(
    Guid Id,
    string NombreCompleto,
    DateOnly FechaNacimiento,
    string Telefono,
    string? Email,
    string? DocumentoIdentidad,
    string? Alergias,
    string? AntecedentesRelevantes,
    string? EnfermedadesCronicas,
    string? EtsIts,
    string? Vicios,
    string? TipoSangre,
    int? Gestas,
    int? Partos,
    int? Cesareas,
    int? Abortos,
    bool Activo,
    DateTime CreatedAtUtc)
{
    public static PatientDto From(Patient patient) => new(
        patient.Id,
        patient.NombreCompleto,
        patient.FechaNacimiento,
        patient.Telefono,
        patient.Email,
        patient.DocumentoIdentidad,
        patient.Alergias,
        patient.AntecedentesRelevantes,
        patient.EnfermedadesCronicas,
        patient.EtsIts,
        patient.Vicios,
        patient.TipoSangre,
        patient.Gestas,
        patient.Partos,
        patient.Cesareas,
        patient.Abortos,
        patient.Activo,
        patient.CreatedAtUtc);
}

public sealed record CreatePatientCommand(
    string NombreCompleto,
    DateOnly FechaNacimiento,
    string Telefono,
    string? Email,
    string? DocumentoIdentidad,
    string? Alergias,
    string? AntecedentesRelevantes,
    string? EnfermedadesCronicas,
    string? EtsIts,
    string? Vicios,
    string? TipoSangre = null,
    int? Gestas = null,
    int? Partos = null,
    int? Cesareas = null,
    int? Abortos = null);

public sealed record UpdatePatientCommand(
    string NombreCompleto,
    DateOnly FechaNacimiento,
    string Telefono,
    string? Email,
    string? DocumentoIdentidad,
    string? Alergias,
    string? AntecedentesRelevantes,
    string? EnfermedadesCronicas,
    string? EtsIts,
    string? Vicios,
    string? TipoSangre = null,
    int? Gestas = null,
    int? Partos = null,
    int? Cesareas = null,
    int? Abortos = null);

public sealed record FamilyMedicalHistoryDto(Guid Id, Guid PatientId, string Condicion, string Parentesco, DateTime CreatedAtUtc)
{
    public static FamilyMedicalHistoryDto From(FamilyMedicalHistory familyMedicalHistory) => new(
        familyMedicalHistory.Id,
        familyMedicalHistory.PatientId,
        familyMedicalHistory.Condicion,
        familyMedicalHistory.Parentesco,
        familyMedicalHistory.CreatedAtUtc);
}

public sealed record PatientListQuery(string? Search, int Page, int PageSize);
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);

public sealed class PatientNotFoundException(Guid id) : KeyNotFoundException($"No existe el paciente {id}.");
public sealed class FamilyMedicalHistoryNotFoundException(Guid id) : KeyNotFoundException($"No existe el antecedente familiar {id}.");
