namespace Ginecologia.Modules.Citas.Application;

public sealed class ListScheduleExceptionsUseCase(IScheduleExceptionRepository scheduleExceptions)
{
    private static readonly TimeZoneInfo ClinicTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Managua");

    public async Task<IReadOnlyList<ScheduleExceptionDto>> ExecuteAsync(DateOnly? desde = null, DateOnly? hasta = null, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ClinicTimeZone));
        var from = desde ?? today;
        var to = hasta ?? from.AddDays(90);
        var items = await scheduleExceptions.ListAsync(from, to, cancellationToken);
        return items.Select(x => new ScheduleExceptionDto(x.Id, x.Fecha, x.Descripcion, x.CreatedAtUtc)).ToList();
    }
}
