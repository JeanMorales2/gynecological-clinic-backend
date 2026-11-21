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
public sealed record PatientBirthdayDto(Guid PatientId, string NombreCompleto, int MesNacimiento, int DiaNacimiento);

public sealed record PatientDto(
    Guid Id,
    string NombreCompleto,
    DateOnly FechaNacimiento,
    string Telefono,
    string? Email,
    string? DocumentoIdentidad,
    string? Alergias,
    string? AntecedentesRelevantes,
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
    string? AntecedentesRelevantes);

public sealed record UpdatePatientCommand(
    string NombreCompleto,
    DateOnly FechaNacimiento,
    string Telefono,
    string? Email,
    string? DocumentoIdentidad,
    string? Alergias,
    string? AntecedentesRelevantes);

public sealed record PatientListQuery(string? Search, int Page, int PageSize);
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);

public sealed class PatientNotFoundException(Guid id) : KeyNotFoundException($"No existe el paciente {id}.");
