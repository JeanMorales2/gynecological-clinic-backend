using Ginecologia.Modules.Citas.Application;
using Ginecologia.Modules.Citas.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.Citas.Infrastructure;

public sealed class AppointmentRepository(CitasDbContext dbContext) : IAppointmentRepository
{
    public Task<Appointment?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Appointments.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<PagedAppointmentResult<Appointment>> ListAsync(AppointmentListQuery query, CancellationToken cancellationToken = default)
    {
        IQueryable<Appointment> appointments = dbContext.Appointments.AsNoTracking();
        if (query.ProfesionalId is { } profesionalId) appointments = appointments.Where(x => x.ProfesionalId == profesionalId);
        if (query.PatientId is { } patientId) appointments = appointments.Where(x => x.PatientId == patientId);
        if (query.Estado is { } estado) appointments = appointments.Where(x => x.Estado == estado);
        if (query.Fecha is { } fecha)
        {
            var start = DateTime.SpecifyKind(fecha.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
            var end = start.AddDays(1);
            appointments = appointments.Where(x => x.FechaHoraInicio >= start && x.FechaHoraInicio < end);
        }

        var totalCount = await appointments.CountAsync(cancellationToken);
        var items = await appointments.OrderBy(x => x.FechaHoraInicio).ThenBy(x => x.CreatedAtUtc)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync(cancellationToken);
        return new PagedAppointmentResult<Appointment>(items, totalCount, query.Page, query.PageSize);
    }

    public Task<bool> HasOverlapAsync(Guid profesionalId, DateTime fechaHoraInicio, DateTime fechaHoraFin, Guid? excludedAppointmentId = null, CancellationToken cancellationToken = default)
    {
        // The maximum duration is 240 minutes. This lower bound lets PostgreSQL use the
        // (profesional_id, fecha_hora_inicio) index before checking the exact interval rule.
        var earliestPossibleStart = fechaHoraInicio.AddMinutes(-240);
        return dbContext.Appointments.AsNoTracking().AnyAsync(x =>
            x.ProfesionalId == profesionalId &&
            (!excludedAppointmentId.HasValue || x.Id != excludedAppointmentId.Value) &&
            x.FechaHoraInicio < fechaHoraFin &&
            x.FechaHoraInicio > earliestPossibleStart &&
            x.FechaHoraInicio.AddMinutes(x.DuracionMinutos) > fechaHoraInicio,
            cancellationToken);
    }

    public Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default) =>
        dbContext.Appointments.AddAsync(appointment, cancellationToken).AsTask();

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
