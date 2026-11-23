namespace Ginecologia.Modules.Promociones.Domain;
public enum PromotionType { Cumpleanos, DiaFestivo, ClienteFrecuente, TerceraEdad }
public enum PromotionStatus { Generada, Enviada, Descartada }
public sealed class Promotion
{
    private Promotion() { }
    private Promotion(Guid id, Guid patientId, PromotionType tipo, string titulo, string mensaje, DateOnly fechaGenerada) { Id = id; PatientId = RequiredId(patientId); Tipo = tipo; Titulo = RequiredText(titulo, nameof(titulo), 250); Mensaje = RequiredText(mensaje, nameof(mensaje), 2000); FechaGenerada = fechaGenerada; Estado = PromotionStatus.Generada; CreatedAtUtc = DateTime.UtcNow; }
    public Guid Id { get; private set; } public Guid PatientId { get; private set; } public PromotionType Tipo { get; private set; } public string Titulo { get; private set; } = string.Empty; public string Mensaje { get; private set; } = string.Empty; public DateOnly FechaGenerada { get; private set; } public PromotionStatus Estado { get; private set; } public DateTime CreatedAtUtc { get; private set; }
    public static Promotion Create(Guid patientId, PromotionType tipo, string titulo, string mensaje, DateOnly fechaGenerada) => new(Guid.NewGuid(), patientId, tipo, titulo, mensaje, fechaGenerada);
    public void MarcarEnviada() { if (Estado != PromotionStatus.Generada) throw new InvalidOperationException("Solo se puede marcar enviada una promoción en estado Generada."); Estado = PromotionStatus.Enviada; }
    public void Descartar() { EnsureNotDescartada(); Estado = PromotionStatus.Descartada; }
    private void EnsureNotDescartada() { if (Estado == PromotionStatus.Descartada) throw new InvalidOperationException("La promoción ya fue descartada."); }
    private static Guid RequiredId(Guid value) => value == Guid.Empty ? throw new ArgumentException("El paciente es obligatorio.", nameof(value)) : value;
    private static string RequiredText(string? value, string name, int max) { var text = value?.Trim(); if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("El campo es obligatorio.", name); if (text.Length > max) throw new ArgumentException($"El campo no puede superar {max} caracteres.", name); return text; }
}
