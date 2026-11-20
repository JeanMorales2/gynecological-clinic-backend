namespace Ginecologia.Modules.Prospeccion.Application;

public sealed class UpdateProspectUseCase(IProspectRepository prospects)
{
    public async Task<ProspectDto> ExecuteAsync(Guid id, UpdateProspectCommand command, CancellationToken cancellationToken = default)
    {
        var prospect = await prospects.FindByIdAsync(id, cancellationToken) ?? throw new ProspectNotFoundException(id);
        prospect.Update(command.NombreCompleto, command.Telefono, command.Email, command.Fuente, command.Estado, command.Notas);
        await prospects.SaveChangesAsync(cancellationToken);
        return ProspectDto.From(prospect);
    }
}
