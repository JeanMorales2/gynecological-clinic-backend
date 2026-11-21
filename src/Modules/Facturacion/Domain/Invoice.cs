namespace Ginecologia.Modules.Facturacion.Domain;

public enum InvoiceStatus { Pendiente, Pagada, Anulada }

public sealed class Invoice
{
    private readonly List<InvoiceItem> _items = [];
    private Invoice() { }
    private Invoice(Guid patientId, Guid? appointmentId, string numeroFactura, DateOnly fecha, IEnumerable<InvoiceItemInput> items, Guid creadoPorId)
    {
        Id = Guid.NewGuid(); PatientId = RequiredId(patientId, nameof(patientId)); AppointmentId = appointmentId; NumeroFactura = RequiredText(numeroFactura, nameof(numeroFactura), 30); Fecha = fecha; CreadoPorId = RequiredId(creadoPorId, nameof(creadoPorId)); Estado = InvoiceStatus.Pendiente;
        var orden = 0; foreach (var item in items) _items.Add(InvoiceItem.Create(item.Concepto, item.Cantidad, item.PrecioUnitario, orden++));
        if (_items.Count == 0) throw new ArgumentException("La factura debe contener al menos un ítem.", nameof(items));
        RecalculateTotal();
    }
    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid? AppointmentId { get; private set; }
    public string NumeroFactura { get; private set; } = string.Empty;
    public DateOnly Fecha { get; private set; }
    public IReadOnlyCollection<InvoiceItem> Items => _items;
    public decimal MontoTotal { get; private set; }
    public InvoiceStatus Estado { get; private set; }
    public string? MetodoPago { get; private set; }
    public DateOnly? FechaPago { get; private set; }
    public Guid CreadoPorId { get; private set; }
    public static Invoice Create(Guid patientId, Guid? appointmentId, string numeroFactura, DateOnly fecha, IEnumerable<InvoiceItemInput> items, Guid creadoPorId) => new(patientId, appointmentId, numeroFactura, fecha, items, creadoPorId);
    public void MarkPaid(string metodoPago) { EnsurePending(); MetodoPago = RequiredText(metodoPago, nameof(metodoPago), 100); FechaPago = DateOnly.FromDateTime(DateTime.UtcNow); Estado = InvoiceStatus.Pagada; }
    public void Void() { EnsurePending(); Estado = InvoiceStatus.Anulada; }
    private void RecalculateTotal() => MontoTotal = _items.Sum(x => x.Cantidad * x.PrecioUnitario);
    private void EnsurePending() { if (Estado != InvoiceStatus.Pendiente) throw new InvalidOperationException("La factura ya está en un estado terminal."); }
    private static Guid RequiredId(Guid value, string name) => value == Guid.Empty ? throw new ArgumentException("El identificador es obligatorio.", name) : value;
    private static string RequiredText(string? value, string name, int max) { var normalized = value?.Trim(); if (string.IsNullOrWhiteSpace(normalized)) throw new ArgumentException("El campo es obligatorio.", name); if (normalized.Length > max) throw new ArgumentException($"El campo no puede superar {max} caracteres.", name); return normalized; }
}

public sealed record InvoiceItemInput(string Concepto, int Cantidad, decimal PrecioUnitario);
public sealed class InvoiceItem
{
    private InvoiceItem() { }
    private InvoiceItem(string concepto, int cantidad, decimal precioUnitario, int orden) { Id = Guid.NewGuid(); Concepto = RequiredText(concepto, nameof(concepto), 300); if (cantidad < 1) throw new ArgumentException("La cantidad debe ser al menos 1.", nameof(cantidad)); if (precioUnitario < 0) throw new ArgumentException("El precio unitario no puede ser negativo.", nameof(precioUnitario)); Cantidad = cantidad; PrecioUnitario = precioUnitario; Orden = orden; }
    public Guid Id { get; private set; }
    public Guid InvoiceId { get; private set; }
    public string Concepto { get; private set; } = string.Empty;
    public int Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public int Orden { get; private set; }
    public static InvoiceItem Create(string concepto, int cantidad, decimal precioUnitario, int orden) => new(concepto, cantidad, precioUnitario, orden);
    private static string RequiredText(string? value, string name, int max) { var normalized = value?.Trim(); if (string.IsNullOrWhiteSpace(normalized)) throw new ArgumentException("El campo es obligatorio.", name); if (normalized.Length > max) throw new ArgumentException($"El campo no puede superar {max} caracteres.", name); return normalized; }
}
