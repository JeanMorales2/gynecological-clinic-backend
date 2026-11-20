namespace Ginecologia.Modules.Prospeccion.Application;

public sealed class GetProspectUseCase(IProspectRepository prospects)
{
    public async Task<ProspectDto?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var prospect = await prospects.FindByIdAsync(id, cancellationToken);
        return prospect is null ? null : ProspectDto.From(prospect);
    }
}
