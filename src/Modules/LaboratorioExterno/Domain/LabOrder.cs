namespace Ginecologia.Modules.LaboratorioExterno.Domain;

public enum LabOrderStatus { Solicitado, Recibido, Cancelado }

public sealed class LabOrder
{
    private LabOrder() { }

    private LabOrder(Guid patientId, string tipoEstudio, string laboratorioNombre, DateOnly fechaSolicitud, string? observaciones, Guid solicitadoPorId)
    {
        Id = Guid.NewGuid();
        PatientId = RequiredId(patientId, nameof(patientId));
        TipoEstudio = RequiredText(tipoEstudio, nameof(tipoEstudio), 300);
        LaboratorioNombre = RequiredText(laboratorioNombre, nameof(laboratorioNombre), 300);
        FechaSolicitud = fechaSolicitud;
        Observaciones = OptionalText(observaciones, 4000);
        SolicitadoPorId = RequiredId(solicitadoPorId, nameof(solicitadoPorId));
        Estado = LabOrderStatus.Solicitado;
    }

    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public string TipoEstudio { get; private set; } = string.Empty;
    public string LaboratorioNombre { get; private set; } = string.Empty;
    public DateOnly FechaSolicitud { get; private set; }
    public LabOrderStatus Estado { get; private set; }
    public string? ResultadoResumen { get; private set; }
    public DateOnly? FechaResultado { get; private set; }
    public string? Observaciones { get; private set; }
    public Guid SolicitadoPorId { get; private set; }

    public static LabOrder Create(Guid patientId, string tipoEstudio, string laboratorioNombre, DateOnly fechaSolicitud, string? observaciones, Guid solicitadoPorId) =>
        new(patientId, tipoEstudio, laboratorioNombre, fechaSolicitud, observaciones, solicitadoPorId);

    public void MarkReceived(string resultadoResumen, string? observaciones)
    {
        EnsurePending();
        ResultadoResumen = RequiredText(resultadoResumen, nameof(resultadoResumen), 8000);
        Observaciones = OptionalText(observaciones, 4000);
        FechaResultado = DateOnly.FromDateTime(DateTime.UtcNow);
        Estado = LabOrderStatus.Recibido;
    }

    public void Cancel() { EnsurePending(); Estado = LabOrderStatus.Cancelado; }
    private void EnsurePending() { if (Estado != LabOrderStatus.Solicitado) throw new InvalidOperationException("La orden de laboratorio ya está en un estado terminal."); }
    private static Guid RequiredId(Guid value, string name) => value == Guid.Empty ? throw new ArgumentException("El identificador es obligatorio.", name) : value;
    private static string RequiredText(string? value, string name, int max) { var normalized = value?.Trim(); if (string.IsNullOrWhiteSpace(normalized)) throw new ArgumentException("El campo es obligatorio.", name); if (normalized.Length > max) throw new ArgumentException($"El campo no puede superar {max} caracteres.", name); return normalized; }
    private static string? OptionalText(string? value, int max) { var normalized = string.IsNullOrWhiteSpace(value) ? null : value.Trim(); if (normalized?.Length > max) throw new ArgumentException($"El campo no puede superar {max} caracteres.", nameof(value)); return normalized; }
}
