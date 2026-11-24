using Ginecologia.Modules.ConsultaMedica.Domain;
using Xunit;

namespace Ginecologia.Modules.Tests.ConsultaMedica;

public class ConsultaTests
{
    private static void ActualizarBorradorValido(Consulta consulta, string motivoConsulta = "Control anual", decimal? pesoKg = null)
    {
        consulta.ActualizarBorrador(null, null, pesoKg, null, null, motivoConsulta, null, null, null, null, null, null, null, null, null, null, null, null, []);
    }

    [Fact]
    public void CrearBorrador_EmpiezaEnEstadoBorrador()
    {
        var consulta = Consulta.CrearBorrador(Guid.NewGuid(), new DateOnly(2026, 11, 24), Guid.NewGuid());

        Assert.Equal(ConsultaEstado.Borrador, consulta.Estado);
        Assert.Null(consulta.FinalizadaAtUtc);
    }

    [Fact]
    public void Finalizar_CambiaElEstadoYRegistraLaFecha()
    {
        var consulta = Consulta.CrearBorrador(Guid.NewGuid(), new DateOnly(2026, 11, 24), Guid.NewGuid());

        consulta.Finalizar();

        Assert.Equal(ConsultaEstado.Finalizada, consulta.Estado);
        Assert.NotNull(consulta.FinalizadaAtUtc);
    }

    [Fact]
    public void Finalizar_DosVeces_Lanza()
    {
        var consulta = Consulta.CrearBorrador(Guid.NewGuid(), new DateOnly(2026, 11, 24), Guid.NewGuid());
        consulta.Finalizar();

        Assert.Throws<ConsultaFinalizadaException>(consulta.Finalizar);
    }

    [Fact]
    public void ActualizarBorrador_DespuesDeFinalizar_Lanza()
    {
        var consulta = Consulta.CrearBorrador(Guid.NewGuid(), new DateOnly(2026, 11, 24), Guid.NewGuid());
        consulta.Finalizar();

        Assert.Throws<ConsultaFinalizadaException>(() => ActualizarBorradorValido(consulta));
    }

    [Fact]
    public void ActualizarBorrador_SinMotivoDeConsulta_Lanza()
    {
        var consulta = Consulta.CrearBorrador(Guid.NewGuid(), new DateOnly(2026, 11, 24), Guid.NewGuid());

        Assert.Throws<ArgumentException>(() => ActualizarBorradorValido(consulta, motivoConsulta: "   "));
    }

    [Fact]
    public void ActualizarBorrador_ConPesoNegativoOCero_Lanza()
    {
        var consulta = Consulta.CrearBorrador(Guid.NewGuid(), new DateOnly(2026, 11, 24), Guid.NewGuid());

        Assert.Throws<ArgumentException>(() => ActualizarBorradorValido(consulta, pesoKg: 0));
    }

    [Fact]
    public void ActualizarBorrador_ConDatosValidos_ActualizaLosCampos()
    {
        var consulta = Consulta.CrearBorrador(Guid.NewGuid(), new DateOnly(2026, 11, 24), Guid.NewGuid());

        ActualizarBorradorValido(consulta, motivoConsulta: "Dolor pélvico", pesoKg: 65.5m);

        Assert.Equal("Dolor pélvico", consulta.MotivoConsulta);
        Assert.Equal(65.5m, consulta.PesoKg);
    }

    [Fact]
    public void AddAttachment_DeOtraConsulta_Lanza()
    {
        var consulta = Consulta.CrearBorrador(Guid.NewGuid(), new DateOnly(2026, 11, 24), Guid.NewGuid());
        var attachment = ConsultaAttachment.Create(Guid.NewGuid(), "informe.pdf", "application/pdf", [1, 2, 3]);

        Assert.Throws<ArgumentException>(() => consulta.AddAttachment(attachment));
    }
}

public class ConsultaAttachmentTests
{
    [Fact]
    public void Create_ConTipoDeContenidoNoPermitido_Lanza()
    {
        Assert.Throws<ArgumentException>(() => ConsultaAttachment.Create(Guid.NewGuid(), "archivo.exe", "application/x-msdownload", [1]));
    }

    [Fact]
    public void Create_ConArchivoDeMasDeDiezMegabytes_Lanza()
    {
        var contenido = new byte[10 * 1024 * 1024 + 1];

        Assert.Throws<ArgumentException>(() => ConsultaAttachment.Create(Guid.NewGuid(), "grande.pdf", "application/pdf", contenido));
    }

    [Fact]
    public void Create_ConDatosValidos_CreaElAdjunto()
    {
        var attachment = ConsultaAttachment.Create(Guid.NewGuid(), "informe.pdf", "application/pdf", [1, 2, 3]);

        Assert.Equal("informe.pdf", attachment.NombreArchivo);
        Assert.Equal("application/pdf", attachment.TipoContenido);
    }
}
