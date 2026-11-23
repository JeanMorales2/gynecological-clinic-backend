using Ginecologia.Modules.Citas.Domain;

namespace Ginecologia.Modules.Citas.Application;

public sealed class CreateScheduleExceptionUseCase(IScheduleExceptionRepository scheduleExceptions)
{
    public async Task<ScheduleExceptionDto> ExecuteAsync(DateOnly fecha, string descripcion, CancellationToken cancellationToken = default)
    {
        if (await scheduleExceptions.FindByDateAsync(fecha, cancellationToken) is not null)
            throw new ArgumentException("Ya existe una excepción de horario para esa fecha.");

        var scheduleException = ScheduleException.Create(fecha, descripcion);
        await scheduleExceptions.AddAsync(scheduleException, cancellationToken);
        await scheduleExceptions.SaveChangesAsync(cancellationToken);
        return new ScheduleExceptionDto(scheduleException.Id, scheduleException.Fecha, scheduleException.Descripcion, scheduleException.CreatedAtUtc);
    }
}
