namespace Ginecologia.Modules.Citas.Domain;

public sealed class ScheduleException
{
    private ScheduleException() { }

    private ScheduleException(Guid id, DateOnly fecha, string descripcion)
    {
        Id = id;
        Fecha = fecha;
        Descripcion = RequiredDescription(descripcion);
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public DateOnly Fecha { get; private set; }
    public string Descripcion { get; private set; } = null!;
    public DateTime CreatedAtUtc { get; private set; }

    public static ScheduleException Create(DateOnly fecha, string descripcion) =>
        new(Guid.NewGuid(), fecha, descripcion);

    private static string RequiredDescription(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("La descripción es obligatoria.", nameof(value));

        return value.Trim();
    }
}
