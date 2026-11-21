using Ginecologia.Modules.HistoriaClinica.Domain;

namespace Ginecologia.Modules.HistoriaClinica.Application;

public interface IClinicalEntryRepository
{
    Task<ClinicalEntry?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedClinicalEntryResult<ClinicalEntry>> ListAsync(ClinicalEntryListQuery query, CancellationToken cancellationToken = default);
    Task AddAsync(ClinicalEntry entry, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public sealed record CreateClinicalEntryCommand(Guid PatientId, ClinicalEntryType Tipo, DateOnly Fecha, string Motivo, string Hallazgos, string? Diagnostico, string? Indicaciones, Guid RegistradoPorId);
public sealed record UpdateClinicalEntryCommand(ClinicalEntryType Tipo, DateOnly Fecha, string Motivo, string Hallazgos, string? Diagnostico, string? Indicaciones);
public sealed record ClinicalEntryListQuery(Guid PatientId, ClinicalEntryType? Tipo, int Page, int PageSize);
public sealed record PagedClinicalEntryResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
public sealed record ClinicalEntryDto(Guid Id, Guid PatientId, ClinicalEntryType Tipo, DateOnly Fecha, string Motivo, string Hallazgos, string? Diagnostico, string? Indicaciones, Guid RegistradoPorId, DateTime CreatedAtUtc)
{
    public static ClinicalEntryDto From(ClinicalEntry entry) => new(entry.Id, entry.PatientId, entry.Tipo, entry.Fecha, entry.Motivo, entry.Hallazgos, entry.Diagnostico, entry.Indicaciones, entry.RegistradoPorId, entry.CreatedAtUtc);
}

public sealed class ClinicalEntryNotFoundException(Guid id) : KeyNotFoundException($"No existe la entrada clÃ­nica {id}.");
