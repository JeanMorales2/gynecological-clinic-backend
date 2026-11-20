using Ginecologia.Modules.Prospeccion.Domain;

namespace Ginecologia.Modules.Prospeccion.Application;

public sealed class CreateProspectUseCase(IProspectRepository prospects)
{
    public async Task<ProspectDto> ExecuteAsync(CreateProspectCommand command, CancellationToken cancellationToken = default)
    {
        var prospect = Prospect.Create(command.NombreCompleto, command.Telefono, command.Email, command.Fuente, command.Notas);
        await prospects.AddAsync(prospect, cancellationToken);
        await prospects.SaveChangesAsync(cancellationToken);
        return ProspectDto.From(prospect);
    }
}
