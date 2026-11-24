using Ginecologia.Modules.Citas.Application;
using Ginecologia.Modules.Citas.Domain;
using Xunit;

namespace Ginecologia.Modules.Tests.Citas;

file sealed class FakeScheduleExceptionRepository : IScheduleExceptionRepository
{
    private readonly HashSet<DateOnly> _exceptionDates = [];
    public void MarkAsException(DateOnly fecha) => _exceptionDates.Add(fecha);
    public Task<ScheduleException?> FindByDateAsync(DateOnly fecha, CancellationToken cancellationToken = default) =>
        Task.FromResult(_exceptionDates.Contains(fecha) ? ScheduleException.Create(fecha, "QA") : null);
    public Task<ScheduleException?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<ScheduleException?>(null);
    public Task<IReadOnlyList<ScheduleException>> ListAsync(DateOnly desde, DateOnly hasta, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<ScheduleException>>([]);
    public Task AddAsync(ScheduleException scheduleException, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void RemoveAsync(ScheduleException scheduleException) { }
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}

public class BusinessHoursValidatorTests
{
    private static readonly TimeZoneInfo ClinicTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Managua");

    private static DateTime ClinicLocalToUtc(int year, int month, int day, int hour, int minute) =>
        TimeZoneInfo.ConvertTimeToUtc(new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Unspecified), ClinicTimeZone);

    // 2026-11-23 es lunes, 2026-11-28 es sabado, 2026-11-22 es domingo.
    [Fact]
    public async Task ValidateAsync_LunesEnHorario_NoLanza()
    {
        var validator = new BusinessHoursValidator(new FakeScheduleExceptionRepository());
        var inicio = ClinicLocalToUtc(2026, 11, 23, 9, 0);
        var fin = ClinicLocalToUtc(2026, 11, 23, 9, 30);

        await validator.ValidateAsync(inicio, fin, CancellationToken.None);
    }

    [Fact]
    public async Task ValidateAsync_AntesDeLaAperturaALasOcho_Lanza()
    {
        var validator = new BusinessHoursValidator(new FakeScheduleExceptionRepository());
        var inicio = ClinicLocalToUtc(2026, 11, 23, 7, 30);
        var fin = ClinicLocalToUtc(2026, 11, 23, 8, 0);

        await Assert.ThrowsAsync<ArgumentException>(() => validator.ValidateAsync(inicio, fin, CancellationToken.None));
    }

    [Fact]
    public async Task ValidateAsync_DespuesDelCierreALasCinco_Lanza()
    {
        var validator = new BusinessHoursValidator(new FakeScheduleExceptionRepository());
        var inicio = ClinicLocalToUtc(2026, 11, 23, 16, 45);
        var fin = ClinicLocalToUtc(2026, 11, 23, 17, 15);

        await Assert.ThrowsAsync<ArgumentException>(() => validator.ValidateAsync(inicio, fin, CancellationToken.None));
    }

    [Fact]
    public async Task ValidateAsync_CruzaElHorarioDeAlmuerzo_Lanza()
    {
        var validator = new BusinessHoursValidator(new FakeScheduleExceptionRepository());
        var inicio = ClinicLocalToUtc(2026, 11, 23, 11, 45);
        var fin = ClinicLocalToUtc(2026, 11, 23, 12, 15);

        await Assert.ThrowsAsync<ArgumentException>(() => validator.ValidateAsync(inicio, fin, CancellationToken.None));
    }

    [Fact]
    public async Task ValidateAsync_SabadoHastaMediodia_NoLanza()
    {
        var validator = new BusinessHoursValidator(new FakeScheduleExceptionRepository());
        var inicio = ClinicLocalToUtc(2026, 11, 28, 11, 30);
        var fin = ClinicLocalToUtc(2026, 11, 28, 12, 0);

        await validator.ValidateAsync(inicio, fin, CancellationToken.None);
    }

    [Fact]
    public async Task ValidateAsync_SabadoDespuesDeMediodia_Lanza()
    {
        var validator = new BusinessHoursValidator(new FakeScheduleExceptionRepository());
        var inicio = ClinicLocalToUtc(2026, 11, 28, 11, 45);
        var fin = ClinicLocalToUtc(2026, 11, 28, 12, 15);

        await Assert.ThrowsAsync<ArgumentException>(() => validator.ValidateAsync(inicio, fin, CancellationToken.None));
    }

    [Fact]
    public async Task ValidateAsync_DomingoSinExcepcion_Lanza()
    {
        var validator = new BusinessHoursValidator(new FakeScheduleExceptionRepository());
        var inicio = ClinicLocalToUtc(2026, 11, 22, 14, 0);
        var fin = ClinicLocalToUtc(2026, 11, 22, 14, 30);

        await Assert.ThrowsAsync<ArgumentException>(() => validator.ValidateAsync(inicio, fin, CancellationToken.None));
    }

    [Fact]
    public async Task ValidateAsync_DomingoConExcepcion_NoLanza()
    {
        var repository = new FakeScheduleExceptionRepository();
        repository.MarkAsException(new DateOnly(2026, 11, 22));
        var validator = new BusinessHoursValidator(repository);
        var inicio = ClinicLocalToUtc(2026, 11, 22, 14, 0);
        var fin = ClinicLocalToUtc(2026, 11, 22, 14, 30);

        await validator.ValidateAsync(inicio, fin, CancellationToken.None);
    }
}
