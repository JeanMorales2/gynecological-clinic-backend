using Ginecologia.Modules.LaboratorioExterno.Application;
using Ginecologia.Modules.LaboratorioExterno.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.LaboratorioExterno.Infrastructure;

public sealed class LabOrderAttachmentRepository(LaboratorioExternoDbContext db) : ILabOrderAttachmentRepository
{
    public Task<LabOrderAttachment?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.LabOrderAttachments.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<LabOrderAttachmentDto>> ListByLabOrderIdAsync(Guid labOrderId, CancellationToken cancellationToken = default) =>
        await db.LabOrderAttachments.AsNoTracking().Where(x => x.LabOrderId == labOrderId).OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new LabOrderAttachmentDto(x.Id, x.LabOrderId, x.NombreArchivo, x.ContentType, x.TamanoBytes, x.SubidoPorId, x.CreatedAtUtc))
            .ToListAsync(cancellationToken);

    public Task AddAsync(LabOrderAttachment attachment, CancellationToken cancellationToken = default) => db.LabOrderAttachments.AddAsync(attachment, cancellationToken).AsTask();
    public Task RemoveAsync(LabOrderAttachment attachment, CancellationToken cancellationToken = default) { db.LabOrderAttachments.Remove(attachment); return Task.CompletedTask; }
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);
}
