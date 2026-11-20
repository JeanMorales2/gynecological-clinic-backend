using Ginecologia.Modules.Prospeccion.Domain;

namespace Ginecologia.Modules.Prospeccion.Application;

public sealed class ListProspectsUseCase(IProspectRepository prospects)
{
    public async Task<PagedProspectResult<ProspectDto>> ExecuteAsync(string? search, ProspectStatus? estado, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = new ProspectListQuery(search?.Trim(), estado, Math.Max(page, 1), Math.Clamp(pageSize <= 0 ? 10 : pageSize, 1, 100));
        var result = await prospects.ListAsync(query, cancellationToken);
        return new PagedProspectResult<ProspectDto>(result.Items.Select(ProspectDto.From).ToArray(), result.TotalCount, result.Page, result.PageSize);
    }
}
