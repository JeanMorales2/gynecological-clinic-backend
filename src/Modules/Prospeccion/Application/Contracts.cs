using Ginecologia.Modules.Prospeccion.Domain;

namespace Ginecologia.Modules.Prospeccion.Application;

public interface IProspectRepository
{
    Task<Prospect?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedProspectResult<Prospect>> ListAsync(ProspectListQuery query, CancellationToken cancellationToken = default);
    Task AddAsync(Prospect prospect, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public sealed record ProspectDto(
    Guid Id,
    string NombreCompleto,
    string Telefono,
    string? TelefonoAlternativo,
    string? Email,
    string? Fuente,
    ProspectStatus Estado,
    string? Notas,
    Guid? PatientId,
    DateTime CreatedAtUtc)
{
    public static ProspectDto From(Prospect prospect) => new(
        prospect.Id,
        prospect.NombreCompleto,
        prospect.Telefono,
        prospect.TelefonoAlternativo,
        prospect.Email,
        prospect.Fuente,
        prospect.Estado,
        prospect.Notas,
        prospect.PatientId,
        prospect.CreatedAtUtc);
}

public sealed record CreateProspectCommand(string NombreCompleto, string Telefono, string? TelefonoAlternativo, string? Email, string? Fuente, string? Notas);
public sealed record UpdateProspectCommand(string NombreCompleto, string Telefono, string? TelefonoAlternativo, string? Email, string? Fuente, ProspectStatus Estado, string? Notas);
public sealed record ConvertProspectCommand(DateOnly FechaNacimiento, string? DocumentoIdentidad, string? Alergias, string? AntecedentesRelevantes);
public sealed record ProspectListQuery(string? Search, ProspectStatus? Estado, int Page, int PageSize);
public sealed record PagedProspectResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);

public sealed class ProspectNotFoundException(Guid id) : KeyNotFoundException($"No existe el prospecto {id}.");
