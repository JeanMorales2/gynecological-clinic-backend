using Ginecologia.Modules.Citas.Application;
using Ginecologia.Modules.Citas.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.Citas.Infrastructure;

public sealed class ScheduleExceptionRepository(CitasDbContext dbContext) : IScheduleExceptionRepository
{
    public Task<ScheduleException?> FindByDateAsync(DateOnly fecha, CancellationToken cancellationToken = default) =>
        dbContext.ScheduleExceptions.SingleOrDefaultAsync(x => x.Fecha == fecha, cancellationToken);

    public Task<ScheduleException?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.ScheduleExceptions.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ScheduleException>> ListAsync(DateOnly desde, DateOnly hasta, CancellationToken cancellationToken = default) =>
        await dbContext.ScheduleExceptions.AsNoTracking()
            .Where(x => x.Fecha >= desde && x.Fecha <= hasta)
            .OrderBy(x => x.Fecha)
            .ToListAsync(cancellationToken);

    public Task AddAsync(ScheduleException scheduleException, CancellationToken cancellationToken = default) =>
        dbContext.ScheduleExceptions.AddAsync(scheduleException, cancellationToken).AsTask();

    public void RemoveAsync(ScheduleException scheduleException) => dbContext.ScheduleExceptions.Remove(scheduleException);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
