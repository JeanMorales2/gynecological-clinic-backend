namespace Ginecologia.Modules.Citas.Domain;

public enum AppointmentStatus
{
    PendienteConsulta,
    Recepcionado,
    EnConsulta,
    Finalizada,
    NoShow,
}

public sealed class Appointment
{
    private Appointment() { }

    private Appointment(Guid id, Guid patientId, Guid profesionalId, DateTime fechaHoraInicio, int duracionMinutos, string? notas)
    {
        Id = id;
        PatientId = RequiredId(patientId, nameof(patientId));
        ProfesionalId = RequiredId(profesionalId, nameof(profesionalId));
        FechaHoraInicio = ValidStart(fechaHoraInicio);
        DuracionMinutos = ValidDuration(duracionMinutos);
        Notas = NormalizeOptional(notas);
        Estado = AppointmentStatus.PendienteConsulta;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid ProfesionalId { get; private set; }
    public DateTime FechaHoraInicio { get; private set; }
    public int DuracionMinutos { get; private set; }
    public AppointmentStatus Estado { get; private set; }
    public string? Notas { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime FechaHoraFin => FechaHoraInicio.AddMinutes(DuracionMinutos);

    public static Appointment Create(Guid patientId, Guid profesionalId, DateTime fechaHoraInicio, int duracionMinutos = 30, string? notas = null) =>
        new(Guid.NewGuid(), patientId, profesionalId, fechaHoraInicio, duracionMinutos, notas);

    public void Update(Guid patientId, Guid profesionalId, DateTime fechaHoraInicio, int duracionMinutos, string? notas)
    {
        if (Estado != AppointmentStatus.PendienteConsulta)
            throw new InvalidOperationException("Solo se pueden editar citas pendientes de consulta.");

        PatientId = RequiredId(patientId, nameof(patientId));
        ProfesionalId = RequiredId(profesionalId, nameof(profesionalId));
        FechaHoraInicio = ValidStart(fechaHoraInicio);
        DuracionMinutos = ValidDuration(duracionMinutos);
        Notas = NormalizeOptional(notas);
    }

    public void ChangeStatus(AppointmentStatus newStatus)
    {
        var isAllowed = (Estado, newStatus) switch
        {
            (AppointmentStatus.PendienteConsulta, AppointmentStatus.Recepcionado) => true,
            (AppointmentStatus.PendienteConsulta, AppointmentStatus.NoShow) => true,
            (AppointmentStatus.Recepcionado, AppointmentStatus.EnConsulta) => true,
            (AppointmentStatus.EnConsulta, AppointmentStatus.Finalizada) => true,
            _ => false,
        };

        if (!isAllowed)
            throw new InvalidOperationException($"No se puede cambiar una cita de {Estado} a {newStatus}.");

        Estado = newStatus;
    }

    private static Guid RequiredId(Guid value, string parameterName)
    {
        if (value == Guid.Empty) throw new ArgumentException("El identificador es obligatorio.", parameterName);
        return value;
    }

    private static DateTime ValidStart(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new ArgumentException("La fecha y hora de inicio debe estar expresada en UTC.", nameof(value));
        if (value < DateTime.UtcNow)
            throw new ArgumentException("No se puede agendar una cita en el pasado.", nameof(value));
        return value;
    }

    private static int ValidDuration(int value)
    {
        if (value is < 5 or > 240)
            throw new ArgumentOutOfRangeException(nameof(value), "La duración debe estar entre 5 y 240 minutos.");
        return value;
    }

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
