using Ginecologia.Modules.Citas.Domain;
using Xunit;

namespace Ginecologia.Modules.Tests.Citas;

public class AppointmentTests
{
    [Fact]
    public void Create_ConFechaFutura_NoLanza()
    {
        var inicio = DateTime.UtcNow.AddDays(1);

        var appointment = Appointment.Create(Guid.NewGuid(), Guid.NewGuid(), inicio, 30);

        Assert.Equal(inicio, appointment.FechaHoraInicio);
    }

    [Fact]
    public void Create_ConFechaPasada_Lanza()
    {
        var inicio = DateTime.UtcNow.AddDays(-1);

        Assert.Throws<ArgumentException>(() => Appointment.Create(Guid.NewGuid(), Guid.NewGuid(), inicio, 30));
    }

    [Fact]
    public void Create_ConFechaNoUtc_Lanza()
    {
        var inicio = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(1), DateTimeKind.Local);

        Assert.Throws<ArgumentException>(() => Appointment.Create(Guid.NewGuid(), Guid.NewGuid(), inicio, 30));
    }

    [Fact]
    public void Update_PendienteConsultaAFechaPasada_Lanza()
    {
        var appointment = Appointment.Create(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), 30);

        Assert.Throws<ArgumentException>(() =>
            appointment.Update(appointment.PatientId, appointment.ProfesionalId, DateTime.UtcNow.AddDays(-1), 30, null));
    }

    [Fact]
    public void Update_PendienteConsultaAFechaFutura_ActualizaInicio()
    {
        var appointment = Appointment.Create(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), 30);
        var nuevoInicio = DateTime.UtcNow.AddDays(2);

        appointment.Update(appointment.PatientId, appointment.ProfesionalId, nuevoInicio, 45, "Reagendada");

        Assert.Equal(nuevoInicio, appointment.FechaHoraInicio);
        Assert.Equal(45, appointment.DuracionMinutos);
    }
}
