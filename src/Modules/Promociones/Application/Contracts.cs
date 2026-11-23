using Ginecologia.Modules.Promociones.Domain;
namespace Ginecologia.Modules.Promociones.Application;
public interface IPromotionRepository { Task<Promotion?> FindByIdAsync(Guid id, CancellationToken ct = default); Task<PagedPromotionResult<Promotion>> ListAsync(PromotionListQuery query, CancellationToken ct = default); Task<bool> ExistsForPatientOnDateAsync(Guid patientId, PromotionType tipo, DateOnly fecha, CancellationToken ct = default); Task AddAsync(Promotion promotion, CancellationToken ct = default); Task RemoveAsync(Promotion promotion, CancellationToken ct = default); Task SaveChangesAsync(CancellationToken ct = default); }
public sealed record CreatePromotionCommand(Guid PatientId, PromotionType Tipo, string Titulo, string Mensaje, DateOnly FechaGenerada);
public sealed record PromotionListQuery(Guid? PatientId, PromotionStatus? Estado, int Page, int PageSize);
public sealed record PagedPromotionResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
public sealed record PromotionDto(Guid Id, Guid PatientId, string? PatientNombreCompleto, PromotionType Tipo, string Titulo, string Mensaje, DateOnly FechaGenerada, PromotionStatus Estado, DateTime CreatedAtUtc) { public static PromotionDto From(Promotion x, string? patientNombreCompleto = null) => new(x.Id, x.PatientId, patientNombreCompleto, x.Tipo, x.Titulo, x.Mensaje, x.FechaGenerada, x.Estado, x.CreatedAtUtc); }
public sealed class PromotionNotFoundException(Guid id) : KeyNotFoundException($"No existe la promoción {id}.");
