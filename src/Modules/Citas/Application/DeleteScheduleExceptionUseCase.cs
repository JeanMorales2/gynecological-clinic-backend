namespace Ginecologia.Modules.Citas.Application;

public sealed class DeleteScheduleExceptionUseCase(IScheduleExceptionRepository scheduleExceptions)
{
    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var scheduleException = await scheduleExceptions.FindByIdAsync(id, cancellationToken)
            ?? throw new ScheduleExceptionNotFoundException(id);
        scheduleExceptions.RemoveAsync(scheduleException);
        await scheduleExceptions.SaveChangesAsync(cancellationToken);
    }
}
