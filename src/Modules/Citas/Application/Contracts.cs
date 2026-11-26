using Ginecologia.Modules.Citas.Domain;

namespace Ginecologia.Modules.Citas.Application;

public interface IAppointmentRepository
{
    Task<Appointment?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedAppointmentResult<Appointment>> ListAsync(AppointmentListQuery query, CancellationToken cancellationToken = default);
    Task<bool> HasOverlapAsync(Guid profesionalId, DateTime fechaHoraInicio, DateTime fechaHoraFin, Guid? excludedAppointmentId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IScheduleExceptionRepository
{
    Task<ScheduleException?> FindByDateAsync(DateOnly fecha, CancellationToken cancellationToken = default);
    Task<ScheduleException?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ScheduleException>> ListAsync(DateOnly desde, DateOnly hasta, CancellationToken cancellationToken = default);
    Task AddAsync(ScheduleException scheduleException, CancellationToken cancellationToken = default);
    void RemoveAsync(ScheduleException scheduleException);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

public sealed record CreateAppointmentCommand(Guid PatientId, Guid ProfesionalId, DateTime FechaHoraInicio, int DuracionMinutos, string? Notas);
public sealed record UpdateAppointmentCommand(Guid PatientId, Guid ProfesionalId, DateTime FechaHoraInicio, int DuracionMinutos, string? Notas);
public sealed record AppointmentListQuery(Guid? ProfesionalId, Guid? PatientId, DateOnly? Fecha, DateOnly? Desde, AppointmentStatus? Estado, int Page, int PageSize);
public sealed record PagedAppointmentResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
public sealed record AppointmentDto(Guid Id, Guid PatientId, string PacienteNombreCompleto, Guid ProfesionalId, string ProfesionalNombreCompleto, DateTime FechaHoraInicio, DateTime FechaHoraFin, int DuracionMinutos, AppointmentStatus Estado, string? Notas, DateTime CreatedAtUtc);
public sealed record ScheduleExceptionDto(Guid Id, DateOnly Fecha, string Descripcion, DateTime CreatedAtUtc);
public sealed class AppointmentNotFoundException(Guid id) : KeyNotFoundException($"No existe la cita {id}.");
public sealed class ScheduleExceptionNotFoundException(Guid id) : KeyNotFoundException($"No existe la excepción de horario {id}.");
