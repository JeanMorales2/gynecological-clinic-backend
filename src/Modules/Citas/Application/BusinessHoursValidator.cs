namespace Ginecologia.Modules.Citas.Application;

public sealed class BusinessHoursValidator(IScheduleExceptionRepository scheduleExceptions)
{
    private static readonly TimeZoneInfo ClinicTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Managua");

    public async Task ValidateAsync(DateTime fechaHoraInicioUtc, DateTime fechaHoraFinUtc, CancellationToken cancellationToken)
    {
        var inicioLocal = TimeZoneInfo.ConvertTimeFromUtc(fechaHoraInicioUtc, ClinicTimeZone);
        var finLocal = TimeZoneInfo.ConvertTimeFromUtc(fechaHoraFinUtc, ClinicTimeZone);

        if (await scheduleExceptions.FindByDateAsync(DateOnly.FromDateTime(inicioLocal), cancellationToken) is not null)
            return;

        if (inicioLocal.DayOfWeek == DayOfWeek.Sunday)
            throw new ArgumentException("La clínica no atiende los domingos. Marcá el día como excepción si es un evento especial.");

        var opening = new TimeOnly(8, 0);
        var closing = inicioLocal.DayOfWeek == DayOfWeek.Saturday ? new TimeOnly(12, 0) : new TimeOnly(17, 0);
        var inicio = TimeOnly.FromDateTime(inicioLocal);
        var fin = TimeOnly.FromDateTime(finLocal);

        if (inicioLocal.Date != finLocal.Date || inicio < opening || fin > closing)
        {
            var schedule = inicioLocal.DayOfWeek == DayOfWeek.Saturday ? "los sábados de 8:00am a 12:00pm" : "de lunes a viernes de 8:00am a 5:00pm";
            throw new ArgumentException($"La cita completa debe estar dentro del horario de atención {schedule}.");
        }

        if (inicioLocal.DayOfWeek is >= DayOfWeek.Monday and <= DayOfWeek.Friday &&
            inicio < new TimeOnly(13, 0) && fin > new TimeOnly(12, 0))
            throw new ArgumentException("La cita no puede superponerse con el horario de almuerzo de 12:00pm a 1:00pm.");
    }
}
