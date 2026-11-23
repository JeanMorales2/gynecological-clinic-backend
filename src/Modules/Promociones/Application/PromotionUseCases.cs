using Ginecologia.Modules.Pacientes.Application;
using Ginecologia.Modules.Notificaciones.Application;
using Ginecologia.Modules.Notificaciones.Domain;
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
public sealed class GenerateBirthdayPromotionsUseCase(IPromotionRepository promotions, ListActivePatientsBirthdaysUseCase birthdays, SendEmailNotificationUseCase sendEmailNotification)
{
    public async Task<int> ExecuteAsync(DateOnly fecha, CancellationToken ct = default)
    {
        var generated = 0; var patients = await birthdays.ExecuteAsync(ct);
        foreach (var patient in patients)
        {
            if (!HasBirthdayOn(patient, fecha) || await promotions.ExistsForPatientOnDateAsync(patient.PatientId, PromotionType.Cumpleanos, fecha, ct)) continue;
            var promotion = Promotion.Create(patient.PatientId, PromotionType.Cumpleanos, $"¡Feliz cumpleaños, {patient.NombreCompleto}!", "Te deseamos un feliz cumpleaños. Consultá por nuestra oferta especial de cumpleaños.", fecha);
            await promotions.AddAsync(promotion, ct);
            if (!string.IsNullOrWhiteSpace(patient.Email))
            {
                var notification = await sendEmailNotification.ExecuteAsync(patient.Email, promotion.Titulo, promotion.Mensaje, ct);
                if (notification.Estado == EmailNotificationStatus.Enviado) promotion.MarcarEnviada();
            }
            generated++;
        }
        if (generated > 0) await promotions.SaveChangesAsync(ct); return generated;
    }
    private static bool HasBirthdayOn(PatientBirthdayDto patient, DateOnly date) => patient.MesNacimiento == 2 && patient.DiaNacimiento == 29 && !DateTime.IsLeapYear(date.Year) ? date.Month == 3 && date.Day == 1 : patient.MesNacimiento == date.Month && patient.DiaNacimiento == date.Day;
}
public sealed class CreatePromotionUseCase(IPromotionRepository promotions, GetPatientUseCase getPatient, SendEmailNotificationUseCase sendEmailNotification)
{
    public async Task<PromotionDto> ExecuteAsync(CreatePromotionCommand command, CancellationToken ct = default)
    {
        var patient = await getPatient.ExecuteAsync(command.PatientId, ct) ?? throw new ArgumentException("El paciente indicado no existe.");
        if (await promotions.ExistsForPatientOnDateAsync(command.PatientId, command.Tipo, command.FechaGenerada, ct))
            throw new ArgumentException("Ya existe una promoción de ese tipo para este paciente en esa fecha.");

        var promotion = Promotion.Create(command.PatientId, command.Tipo, command.Titulo, command.Mensaje, command.FechaGenerada);
        await promotions.AddAsync(promotion, ct);
        if (!string.IsNullOrWhiteSpace(patient.Email))
        {
            var notification = await sendEmailNotification.ExecuteAsync(patient.Email, promotion.Titulo, promotion.Mensaje, ct);
            if (notification.Estado == EmailNotificationStatus.Enviado) promotion.MarcarEnviada();
        }
        await promotions.SaveChangesAsync(ct);
        return PromotionDto.From(promotion, patient.NombreCompleto);
    }
}
public sealed class DiscardPromotionUseCase(IPromotionRepository promotions, GetPatientUseCase getPatient) { public async Task<PromotionDto> ExecuteAsync(Guid id, CancellationToken ct = default) { var promotion = await promotions.FindByIdAsync(id, ct) ?? throw new PromotionNotFoundException(id); promotion.Descartar(); await promotions.SaveChangesAsync(ct); return await promotion.ToDtoWithPatientNameAsync(getPatient, ct); } }
public sealed class DeletePromotionUseCase(IPromotionRepository promotions)
{
    public async Task ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var promotion = await promotions.FindByIdAsync(id, ct) ?? throw new PromotionNotFoundException(id);
        if (promotion.Estado != PromotionStatus.Descartada) throw new InvalidOperationException("Solo se pueden eliminar promociones descartadas.");
        await promotions.RemoveAsync(promotion, ct);
        await promotions.SaveChangesAsync(ct);
    }
}
public sealed class GetPromotionUseCase(IPromotionRepository promotions, GetPatientUseCase getPatient) { public async Task<PromotionDto?> ExecuteAsync(Guid id, CancellationToken ct = default) { var item = await promotions.FindByIdAsync(id, ct); return item is null ? null : await item.ToDtoWithPatientNameAsync(getPatient, ct); } }
public sealed class ListPromotionsUseCase(IPromotionRepository promotions, GetPatientUseCase getPatient) { public async Task<PagedPromotionResult<PromotionDto>> ExecuteAsync(Guid? patientId, PromotionStatus? estado, int page, int pageSize, CancellationToken ct = default) { if (page < 1 || pageSize is < 1 or > 100) throw new ArgumentException("La paginación es inválida."); var result = await promotions.ListAsync(new(patientId, estado, page, pageSize), ct); var dtos = new List<PromotionDto>(result.Items.Count); foreach (var item in result.Items) dtos.Add(await item.ToDtoWithPatientNameAsync(getPatient, ct)); return new(dtos, result.TotalCount, result.Page, result.PageSize); } }
