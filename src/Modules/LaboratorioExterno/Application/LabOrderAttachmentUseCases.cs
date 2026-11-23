using Ginecologia.Modules.LaboratorioExterno.Domain;

namespace Ginecologia.Modules.LaboratorioExterno.Application;

public sealed class UploadLabOrderAttachmentUseCase(ILabOrderRepository orders, ILabOrderAttachmentRepository attachments)
{
    public async Task<LabOrderAttachmentDto> ExecuteAsync(Guid labOrderId, string nombreArchivo, string contentType, byte[] contenido, Guid subidoPorId, CancellationToken cancellationToken = default)
    {
        if (await orders.FindByIdAsync(labOrderId, cancellationToken) is null) throw new LabOrderNotFoundException(labOrderId);
        var attachment = LabOrderAttachment.Create(labOrderId, nombreArchivo, contentType, contenido, subidoPorId);
        await attachments.AddAsync(attachment, cancellationToken);
        await attachments.SaveChangesAsync(cancellationToken);
        return LabOrderAttachmentDto.From(attachment);
    }
}

public sealed class ListLabOrderAttachmentsUseCase(ILabOrderAttachmentRepository attachments)
{
    public Task<IReadOnlyList<LabOrderAttachmentDto>> ExecuteAsync(Guid labOrderId, CancellationToken cancellationToken = default) =>
        attachments.ListByLabOrderIdAsync(labOrderId, cancellationToken);
}

public sealed class DeleteLabOrderAttachmentUseCase(ILabOrderAttachmentRepository attachments)
{
    public async Task ExecuteAsync(Guid attachmentId, CancellationToken cancellationToken = default)
    {
        var attachment = await attachments.FindByIdAsync(attachmentId, cancellationToken) ?? throw new KeyNotFoundException();
        await attachments.RemoveAsync(attachment, cancellationToken);
        await attachments.SaveChangesAsync(cancellationToken);
    }
}

public sealed class GetLabOrderAttachmentContentUseCase(ILabOrderAttachmentRepository attachments)
{
    public async Task<LabOrderAttachmentContent?> ExecuteAsync(Guid attachmentId, CancellationToken cancellationToken = default)
    {
        var attachment = await attachments.FindByIdAsync(attachmentId, cancellationToken);
        return attachment is null ? null : new LabOrderAttachmentContent(attachment.Contenido, attachment.ContentType, attachment.NombreArchivo);
    }
}
