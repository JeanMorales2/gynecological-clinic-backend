using Ginecologia.Modules.Notificaciones.Application;
using Ginecologia.Modules.Notificaciones.Domain;

namespace Ginecologia.Modules.Prospeccion.Application;

public sealed class InviteProspectUseCase(IProspectRepository prospects, SendEmailNotificationUseCase sendEmailNotification)
{
    public async Task<ProspectDto> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var prospect = await prospects.FindByIdAsync(id, cancellationToken) ?? throw new ProspectNotFoundException(id);
        if (string.IsNullOrWhiteSpace(prospect.Email))
            throw new ArgumentException("El prospecto no tiene correo electrónico cargado.");

        var notification = await sendEmailNotification.ExecuteAsync(
            prospect.Email,
            "Te invitamos a conocernos",
            $"Hola {prospect.NombreCompleto}, te invitamos a agendar tu primera consulta con nosotros. Responde este correo para coordinar.",
            cancellationToken);

        if (notification.Estado == EmailNotificationStatus.Enviado) prospect.MarcarContactado();
        await prospects.SaveChangesAsync(cancellationToken);
        return ProspectDto.From(prospect);
    }
}
