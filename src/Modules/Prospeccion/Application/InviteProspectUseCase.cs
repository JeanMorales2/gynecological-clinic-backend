using Ginecologia.Modules.Notificaciones.Application;
using Ginecologia.Modules.Notificaciones.Domain;

namespace Ginecologia.Modules.Prospeccion.Application;

public sealed record InviteProspectResult(ProspectDto Prospect, bool EmailEnviado, string? ErrorEnvio);

public sealed class InviteProspectUseCase(IProspectRepository prospects, SendEmailNotificationUseCase sendEmailNotification)
{
    public async Task<InviteProspectResult> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var prospect = await prospects.FindByIdAsync(id, cancellationToken) ?? throw new ProspectNotFoundException(id);
        if (string.IsNullOrWhiteSpace(prospect.Email))
            throw new ArgumentException("El prospecto no tiene correo electrónico cargado.");

        var notification = await sendEmailNotification.ExecuteAsync(
            prospect.Email,
            "Te invitamos a conocernos",
            $"Hola {prospect.NombreCompleto}, te invitamos a agendar tu primera consulta con nosotros. Responde este correo para coordinar.",
            cancellationToken);

        var enviado = notification.Estado == EmailNotificationStatus.Enviado;
        if (enviado) prospect.MarcarContactado();
        await prospects.SaveChangesAsync(cancellationToken);
        return new InviteProspectResult(ProspectDto.From(prospect), enviado, enviado ? null : notification.UltimoError);
    }
}
