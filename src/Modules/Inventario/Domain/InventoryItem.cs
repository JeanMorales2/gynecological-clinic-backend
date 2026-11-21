namespace Ginecologia.Modules.Inventario.Domain;

public sealed class InventoryItem
{
    private InventoryItem() { }
    private InventoryItem(Guid id, string nombre, string categoria, string unidadMedida, int stockActual, int stockMinimo, decimal? precioUnitario)
    {
        Id = id; Nombre = RequiredText(nombre, nameof(nombre), 150); Categoria = RequiredText(categoria, nameof(categoria), 80);
        UnidadMedida = RequiredText(unidadMedida, nameof(unidadMedida), 30); StockActual = NonNegative(stockActual, nameof(stockActual));
        StockMinimo = NonNegative(stockMinimo, nameof(stockMinimo)); PrecioUnitario = NonNegative(precioUnitario, nameof(precioUnitario));
        Activo = true; CreatedAtUtc = UpdatedAtUtc = DateTime.UtcNow;
    }
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Categoria { get; private set; } = string.Empty;
    public string UnidadMedida { get; private set; } = string.Empty;
    public int StockActual { get; private set; }
    public int StockMinimo { get; private set; }
    public decimal? PrecioUnitario { get; private set; }
    public bool Activo { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public static InventoryItem Create(string nombre, string categoria, string unidadMedida, int stockActual, int stockMinimo, decimal? precioUnitario) =>
        new(Guid.NewGuid(), nombre, categoria, unidadMedida, stockActual, stockMinimo, precioUnitario);
    public void AjustarStock(int delta, string motivo)
    {
        if (!Activo) throw new InvalidOperationException("No se pueden registrar movimientos en un ítem desactivado.");
        if (StockActual + delta < 0) throw new InvalidOperationException("El ajuste dejaría el stock en negativo.");
        StockActual += delta; UpdatedAtUtc = DateTime.UtcNow;
    }
    public void Update(string nombre, string categoria, string unidadMedida, int stockMinimo, decimal? precioUnitario)
    { Nombre = RequiredText(nombre, nameof(nombre), 150); Categoria = RequiredText(categoria, nameof(categoria), 80); UnidadMedida = RequiredText(unidadMedida, nameof(unidadMedida), 30); StockMinimo = NonNegative(stockMinimo, nameof(stockMinimo)); PrecioUnitario = NonNegative(precioUnitario, nameof(precioUnitario)); UpdatedAtUtc = DateTime.UtcNow; }
    public void Desactivar() { Activo = false; UpdatedAtUtc = DateTime.UtcNow; }
    public void Reactivar() { Activo = true; UpdatedAtUtc = DateTime.UtcNow; }
    private static string RequiredText(string? value, string name, int max) { var text = value?.Trim(); if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("El campo es obligatorio.", name); if (text.Length > max) throw new ArgumentException($"El campo no puede superar {max} caracteres.", name); return text; }
    private static int NonNegative(int value, string name) { if (value < 0) throw new ArgumentException("El valor no puede ser negativo.", name); return value; }
    private static decimal? NonNegative(decimal? value, string name) { if (value is < 0) throw new ArgumentException("El valor no puede ser negativo.", name); return value; }
}

public sealed class StockMovement
{
    private StockMovement() { }
    private StockMovement(Guid id, Guid itemId, int delta, string motivo, int stockResultante, Guid registradoPorId)
    { Id = id; InventoryItemId = itemId; Delta = delta; Motivo = RequiredMotivo(motivo); StockResultante = stockResultante; RegistradoPorId = registradoPorId; CreatedAtUtc = DateTime.UtcNow; }
    public Guid Id { get; private set; }
    public Guid InventoryItemId { get; private set; }
    public int Delta { get; private set; }
    public string Motivo { get; private set; } = string.Empty;
    public int StockResultante { get; private set; }
    public Guid RegistradoPorId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public static StockMovement Create(Guid itemId, int delta, string motivo, int stockResultante, Guid registradoPorId) => new(Guid.NewGuid(), itemId, delta, motivo, stockResultante, registradoPorId);
    private static string RequiredMotivo(string? value) { var text = value?.Trim(); if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("El motivo es obligatorio.", nameof(value)); if (text.Length > 200) throw new ArgumentException("El motivo no puede superar 200 caracteres.", nameof(value)); return text; }
}
