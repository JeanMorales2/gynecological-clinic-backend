using Ginecologia.Modules.Pacientes.Application;
using Ginecologia.Modules.Promociones.Domain;
namespace Ginecologia.Modules.Promociones.Application;
internal static class PromotionDtoExtensions
{
    public static async Task<PromotionDto> ToDtoWithPatientNameAsync(this Promotion promotion, GetPatientUseCase getPatient, CancellationToken ct)
    {
        var patient = await getPatient.ExecuteAsync(promotion.PatientId, ct);
        return PromotionDto.From(promotion, patient?.NombreCompleto);
    }
}
public sealed class GenerateBirthdayPromotionsUseCase(IPromotionRepository promotions, ListActivePatientsBirthdaysUseCase birthdays)
{
    public async Task<int> ExecuteAsync(DateOnly fecha, CancellationToken ct = default)
    {
        var generated = 0; var patients = await birthdays.ExecuteAsync(ct);
        foreach (var patient in patients)
        {
            if (!HasBirthdayOn(patient, fecha) || await promotions.ExistsForPatientOnDateAsync(patient.PatientId, PromotionType.Cumpleanos, fecha, ct)) continue;
            var promotion = Promotion.Create(patient.PatientId, PromotionType.Cumpleanos, $"¡Feliz cumpleaños, {patient.NombreCompleto}!", "Te deseamos un feliz cumpleaños. Consultá por nuestra oferta especial de cumpleaños.", fecha);
            await promotions.AddAsync(promotion, ct); generated++;
        }
        if (generated > 0) await promotions.SaveChangesAsync(ct); return generated;
    }
    private static bool HasBirthdayOn(PatientBirthdayDto patient, DateOnly date) => patient.MesNacimiento == 2 && patient.DiaNacimiento == 29 && !DateTime.IsLeapYear(date.Year) ? date.Month == 3 && date.Day == 1 : patient.MesNacimiento == date.Month && patient.DiaNacimiento == date.Day;
}
public sealed class DiscardPromotionUseCase(IPromotionRepository promotions, GetPatientUseCase getPatient) { public async Task<PromotionDto> ExecuteAsync(Guid id, CancellationToken ct = default) { var promotion = await promotions.FindByIdAsync(id, ct) ?? throw new PromotionNotFoundException(id); promotion.Descartar(); await promotions.SaveChangesAsync(ct); return await promotion.ToDtoWithPatientNameAsync(getPatient, ct); } }
public sealed class GetPromotionUseCase(IPromotionRepository promotions, GetPatientUseCase getPatient) { public async Task<PromotionDto?> ExecuteAsync(Guid id, CancellationToken ct = default) { var item = await promotions.FindByIdAsync(id, ct); return item is null ? null : await item.ToDtoWithPatientNameAsync(getPatient, ct); } }
public sealed class ListPromotionsUseCase(IPromotionRepository promotions, GetPatientUseCase getPatient) { public async Task<PagedPromotionResult<PromotionDto>> ExecuteAsync(Guid? patientId, PromotionStatus? estado, int page, int pageSize, CancellationToken ct = default) { if (page < 1 || pageSize is < 1 or > 100) throw new ArgumentException("La paginación es inválida."); var result = await promotions.ListAsync(new(patientId, estado, page, pageSize), ct); var dtos = new List<PromotionDto>(result.Items.Count); foreach (var item in result.Items) dtos.Add(await item.ToDtoWithPatientNameAsync(getPatient, ct)); return new(dtos, result.TotalCount, result.Page, result.PageSize); } }
