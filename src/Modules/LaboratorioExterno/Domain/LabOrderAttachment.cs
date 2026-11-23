namespace Ginecologia.Modules.LaboratorioExterno.Domain;

public sealed class LabOrderAttachment
{
    private const int MaxSizeBytes = 10 * 1024 * 1024;
    private static readonly HashSet<string> AllowedContentTypes = ["application/pdf", "image/jpeg", "image/png"];

    private LabOrderAttachment() { }

    private LabOrderAttachment(Guid labOrderId, string nombreArchivo, string contentType, byte[] contenido, Guid subidoPorId)
    {
        if (labOrderId == Guid.Empty) throw new ArgumentException("La orden de laboratorio es obligatoria.", nameof(labOrderId));
        if (subidoPorId == Guid.Empty) throw new ArgumentException("El usuario que sube el archivo es obligatorio.", nameof(subidoPorId));
        if (string.IsNullOrWhiteSpace(nombreArchivo)) throw new ArgumentException("El nombre del archivo es obligatorio.", nameof(nombreArchivo));
        if (nombreArchivo.Trim().Length > 260) throw new ArgumentException("El nombre del archivo no puede superar 260 caracteres.", nameof(nombreArchivo));
        if (!AllowedContentTypes.Contains(contentType)) throw new ArgumentException("Solo se permiten archivos PDF, JPEG o PNG.", nameof(contentType));
        if (contenido.Length > MaxSizeBytes) throw new ArgumentException("El archivo no puede superar 10 MB.", nameof(contenido));

        Id = Guid.NewGuid();
        LabOrderId = labOrderId;
        NombreArchivo = nombreArchivo.Trim();
        ContentType = contentType;
        Contenido = contenido;
        TamanoBytes = contenido.Length;
        SubidoPorId = subidoPorId;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid LabOrderId { get; private set; }
    public string NombreArchivo { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public byte[] Contenido { get; private set; } = [];
    public int TamanoBytes { get; private set; }
    public Guid SubidoPorId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static LabOrderAttachment Create(Guid labOrderId, string nombreArchivo, string contentType, byte[] contenido, Guid subidoPorId) =>
        new(labOrderId, nombreArchivo, contentType, contenido, subidoPorId);
}
