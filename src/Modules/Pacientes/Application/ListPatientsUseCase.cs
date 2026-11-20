namespace Ginecologia.Modules.Pacientes.Application;

public sealed class ListPatientsUseCase(IPatientRepository patients)
{
    public async Task<PagedResult<PatientDto>> ExecuteAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = new PatientListQuery(search?.Trim(), Math.Max(page, 1), Math.Clamp(pageSize <= 0 ? 10 : pageSize, 1, 100));
        var result = await patients.ListAsync(query, cancellationToken);
        return new PagedResult<PatientDto>(result.Items.Select(PatientDto.From).ToArray(), result.TotalCount, result.Page, result.PageSize);
    }
}
