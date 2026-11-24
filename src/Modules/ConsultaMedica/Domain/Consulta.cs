namespace Ginecologia.Modules.ConsultaMedica.Domain;

public enum ConsultaEstado { Borrador, Finalizada }
public sealed class ConsultaFinalizadaException : InvalidOperationException { public ConsultaFinalizadaException() : base("La consulta ya está finalizada y no puede modificarse.") { } }
public sealed record PrescriptionItemInput(int Orden, string Medicamento, string Dosis, string Via, string Frecuencia, string DuracionTexto);

public sealed class Consulta
{
    private readonly List<PrescriptionItem> _prescriptionItems = [];
    private readonly List<ConsultaAttachment> _attachments = [];
    private Consulta() { }
    private Consulta(Guid patientId, DateOnly fecha, Guid registradoPorId)
    { Id = Guid.NewGuid(); PatientId = RequiredId(patientId, nameof(patientId)); Fecha = fecha; RegistradoPorId = RequiredId(registradoPorId, nameof(registradoPorId)); Estado = ConsultaEstado.Borrador; CreatedAtUtc = UpdatedAtUtc = DateTime.UtcNow; }
    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public DateOnly Fecha { get; private set; }
    public ConsultaEstado Estado { get; private set; }
    public Guid RegistradoPorId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public DateTime? FinalizadaAtUtc { get; private set; }
    public int? PresionSistolica { get; private set; }
    public int? PresionDiastolica { get; private set; }
    public decimal? PesoKg { get; private set; }
    public decimal? TallaCm { get; private set; }
    public DateOnly? Fum { get; private set; }
    public string MotivoConsulta { get; private set; } = string.Empty;
    public string? PadecimientoActual { get; private set; }
    public string? HallazgosMamas { get; private set; }
    public string? HallazgosCervixGenitales { get; private set; }
    public string? HallazgosTactoBimanual { get; private set; }
    public decimal? UltrasonidoEndometrioMm { get; private set; }
    public string? UltrasonidoOvarioDerecho { get; private set; }
    public string? UltrasonidoOvarioIzquierdo { get; private set; }
    public string? UltrasonidoSacoGestacionalEmbrion { get; private set; }
    public string? DiagnosticoCie { get; private set; }
    public string? DiagnosticoDescripcion { get; private set; }
    public string? EstudiosSolicitados { get; private set; }
    public string? IndicacionesProximaCita { get; private set; }
    public IReadOnlyList<PrescriptionItem> PrescriptionItems => _prescriptionItems;
    public IReadOnlyList<ConsultaAttachment> Attachments => _attachments;
    public static Consulta CrearBorrador(Guid patientId, DateOnly fecha, Guid registradoPorId) => new(patientId, fecha, registradoPorId);
    public void ActualizarBorrador(int? presionSistolica, int? presionDiastolica, decimal? pesoKg, decimal? tallaCm, DateOnly? fum, string motivoConsulta, string? padecimientoActual, string? hallazgosMamas, string? hallazgosCervixGenitales, string? hallazgosTactoBimanual, decimal? ultrasonidoEndometrioMm, string? ultrasonidoOvarioDerecho, string? ultrasonidoOvarioIzquierdo, string? ultrasonidoSacoGestacionalEmbrion, string? diagnosticoCie, string? diagnosticoDescripcion, string? estudiosSolicitados, string? indicacionesProximaCita, IReadOnlyList<PrescriptionItemInput> prescriptionItems)
    {
        EnsureDraft();
        PresionSistolica = presionSistolica; PresionDiastolica = presionDiastolica; PesoKg = Positive(pesoKg, nameof(pesoKg)); TallaCm = Positive(tallaCm, nameof(tallaCm)); Fum = fum;
        MotivoConsulta = RequiredText(motivoConsulta, nameof(motivoConsulta), 500); PadecimientoActual = OptionalText(padecimientoActual, 8000);
        HallazgosMamas = OptionalText(hallazgosMamas, 2000); HallazgosCervixGenitales = OptionalText(hallazgosCervixGenitales, 2000); HallazgosTactoBimanual = OptionalText(hallazgosTactoBimanual, 2000);
        UltrasonidoEndometrioMm = Positive(ultrasonidoEndometrioMm, nameof(ultrasonidoEndometrioMm)); UltrasonidoOvarioDerecho = OptionalText(ultrasonidoOvarioDerecho, 1000); UltrasonidoOvarioIzquierdo = OptionalText(ultrasonidoOvarioIzquierdo, 1000); UltrasonidoSacoGestacionalEmbrion = OptionalText(ultrasonidoSacoGestacionalEmbrion, 1000);
        DiagnosticoCie = OptionalText(diagnosticoCie, 10); DiagnosticoDescripcion = OptionalText(diagnosticoDescripcion, 2000); EstudiosSolicitados = OptionalText(estudiosSolicitados, 2000); IndicacionesProximaCita = OptionalText(indicacionesProximaCita, 2000);
        _prescriptionItems.Clear(); foreach (var item in prescriptionItems ?? throw new ArgumentNullException(nameof(prescriptionItems))) _prescriptionItems.Add(PrescriptionItem.Create(Id, item.Orden, item.Medicamento, item.Dosis, item.Via, item.Frecuencia, item.DuracionTexto));
        UpdatedAtUtc = DateTime.UtcNow;
    }
    public void Finalizar() { EnsureDraft(); Estado = ConsultaEstado.Finalizada; FinalizadaAtUtc = UpdatedAtUtc = DateTime.UtcNow; }
    public void AddAttachment(ConsultaAttachment attachment) { EnsureDraft(); if (attachment.ConsultaId != Id) throw new ArgumentException("El adjunto no pertenece a esta consulta."); _attachments.Add(attachment); UpdatedAtUtc = DateTime.UtcNow; }
    public void RemoveAttachment(ConsultaAttachment attachment) { EnsureDraft(); _attachments.Remove(attachment); UpdatedAtUtc = DateTime.UtcNow; }
    private void EnsureDraft() { if (Estado != ConsultaEstado.Borrador) throw new ConsultaFinalizadaException(); }
    private static Guid RequiredId(Guid value, string name) => value == Guid.Empty ? throw new ArgumentException("El identificador es obligatorio.", name) : value;
    private static decimal? Positive(decimal? value, string name) { if (value is <= 0) throw new ArgumentException("El valor debe ser mayor que cero.", name); return value; }
    private static string RequiredText(string? value, string name, int max) { var normalized = value?.Trim(); if (string.IsNullOrWhiteSpace(normalized)) throw new ArgumentException("El campo es obligatorio.", name); if (normalized.Length > max) throw new ArgumentException($"El campo no puede superar {max} caracteres.", name); return normalized; }
    private static string? OptionalText(string? value, int max) { var normalized = string.IsNullOrWhiteSpace(value) ? null : value.Trim(); if (normalized?.Length > max) throw new ArgumentException($"El campo no puede superar {max} caracteres.", nameof(value)); return normalized; }
}

public sealed class PrescriptionItem
{ private PrescriptionItem() { } private PrescriptionItem(Guid consultaId, int orden, string medicamento, string dosis, string via, string frecuencia, string duracionTexto) { Id = Guid.NewGuid(); ConsultaId = consultaId; Orden = orden; Medicamento = Required(medicamento, nameof(medicamento), 200); Dosis = Required(dosis, nameof(dosis), 100); Via = Required(via, nameof(via), 50); Frecuencia = Required(frecuencia, nameof(frecuencia), 100); DuracionTexto = Required(duracionTexto, nameof(duracionTexto), 100); } public Guid Id { get; private set; } public Guid ConsultaId { get; private set; } public int Orden { get; private set; } public string Medicamento { get; private set; } = string.Empty; public string Dosis { get; private set; } = string.Empty; public string Via { get; private set; } = string.Empty; public string Frecuencia { get; private set; } = string.Empty; public string DuracionTexto { get; private set; } = string.Empty; public static PrescriptionItem Create(Guid consultaId, int orden, string medicamento, string dosis, string via, string frecuencia, string duracionTexto) => new(consultaId, orden, medicamento, dosis, via, frecuencia, duracionTexto); private static string Required(string? value, string name, int max) { var text = value?.Trim(); if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("El campo es obligatorio.", name); if (text.Length > max) throw new ArgumentException($"El campo no puede superar {max} caracteres.", name); return text; } }

public sealed class ConsultaAttachment
{ private const int MaxSizeBytes = 10 * 1024 * 1024; private static readonly HashSet<string> AllowedContentTypes = ["application/pdf", "image/jpeg", "image/png"]; private ConsultaAttachment() { } private ConsultaAttachment(Guid consultaId, string nombreArchivo, string tipoContenido, byte[] contenido) { if (consultaId == Guid.Empty) throw new ArgumentException("La consulta es obligatoria.", nameof(consultaId)); if (string.IsNullOrWhiteSpace(nombreArchivo)) throw new ArgumentException("El nombre del archivo es obligatorio.", nameof(nombreArchivo)); if (nombreArchivo.Trim().Length > 260) throw new ArgumentException("El nombre del archivo no puede superar 260 caracteres.", nameof(nombreArchivo)); if (!AllowedContentTypes.Contains(tipoContenido)) throw new ArgumentException("Solo se permiten archivos PDF, JPEG o PNG.", nameof(tipoContenido)); if (contenido.Length > MaxSizeBytes) throw new ArgumentException("El archivo no puede superar 10 MB.", nameof(contenido)); Id = Guid.NewGuid(); ConsultaId = consultaId; NombreArchivo = nombreArchivo.Trim(); TipoContenido = tipoContenido; Contenido = contenido; CreatedAtUtc = DateTime.UtcNow; } public Guid Id { get; private set; } public Guid ConsultaId { get; private set; } public string NombreArchivo { get; private set; } = string.Empty; public string TipoContenido { get; private set; } = string.Empty; public byte[] Contenido { get; private set; } = []; public DateTime CreatedAtUtc { get; private set; } public static ConsultaAttachment Create(Guid consultaId, string nombreArchivo, string tipoContenido, byte[] contenido) => new(consultaId, nombreArchivo, tipoContenido, contenido); }
