using System.Globalization;
using System.Text;
using Ginecologia.Modules.ConsultaMedica.Domain;
using Ginecologia.Modules.Pacientes.Application;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
namespace Ginecologia.Modules.ConsultaMedica.Application;
public sealed class CreateConsultaUseCase(IConsultaRepository consultas, GetPatientUseCase getPatient) { public async Task<ConsultaDto> ExecuteAsync(Guid patientId, DateOnly fecha, Guid userId, CancellationToken ct = default) { if (await getPatient.ExecuteAsync(patientId, ct) is null) throw new ArgumentException("El paciente indicado no existe.", nameof(patientId)); var consulta = Consulta.CrearBorrador(patientId, fecha, userId); await consultas.AddAsync(consulta,ct); await consultas.SaveChangesAsync(ct); return ConsultaDto.From(consulta); } }
public sealed class UpdateConsultaUseCase(IConsultaRepository consultas) { public async Task<ConsultaDto> ExecuteAsync(Guid id, UpdateConsultaCommand command, CancellationToken ct = default) { var consulta=await consultas.FindByIdAsync(id,ct)??throw new ConsultaNotFoundException(id); consulta.ActualizarBorrador(command.PresionSistolica,command.PresionDiastolica,command.PesoKg,command.TallaCm,command.Fum,command.MotivoConsulta,command.PadecimientoActual,command.HallazgosMamas,command.HallazgosCervixGenitales,command.HallazgosTactoBimanual,command.UltrasonidoEndometrioMm,command.UltrasonidoOvarioDerecho,command.UltrasonidoOvarioIzquierdo,command.UltrasonidoSacoGestacionalEmbrion,command.DiagnosticoCie,command.DiagnosticoDescripcion,command.EstudiosSolicitados,command.IndicacionesProximaCita,command.PrescriptionItems); await consultas.SaveChangesAsync(ct); return ConsultaDto.From(consulta); } }
public sealed class FinalizeConsultaUseCase(IConsultaRepository consultas) { public async Task<ConsultaDto> ExecuteAsync(Guid id, CancellationToken ct=default) { var consulta=await consultas.FindByIdAsync(id,ct)??throw new ConsultaNotFoundException(id); consulta.Finalizar(); await consultas.SaveChangesAsync(ct); return ConsultaDto.From(consulta); } }
public sealed class GetConsultaUseCase(IConsultaRepository consultas) { public async Task<ConsultaDto?> ExecuteAsync(Guid id,CancellationToken ct=default) { var c=await consultas.FindByIdAsync(id,ct); return c is null?null:ConsultaDto.From(c); } }
public sealed class ListConsultasUseCase(IConsultaRepository consultas) { public Task<PagedConsultaResult> ExecuteAsync(Guid patientId,int page,int pageSize,CancellationToken ct=default) { if(patientId==Guid.Empty)throw new ArgumentException("El paciente es obligatorio.",nameof(patientId)); if(page<1||pageSize<1||pageSize>100)throw new ArgumentException("Paginación inválida."); return consultas.ListAsync(patientId,page,pageSize,ct); } }
public sealed class UploadConsultaAttachmentUseCase(IConsultaRepository consultas) { public async Task<ConsultaAttachmentDto> ExecuteAsync(Guid id,string name,string type,byte[] content,CancellationToken ct=default) { var c=await consultas.FindByIdAsync(id,ct)??throw new ConsultaNotFoundException(id); var attachment=ConsultaAttachment.Create(id,name,type,content); c.AddAttachment(attachment); await consultas.SaveChangesAsync(ct); return ConsultaAttachmentDto.From(attachment); } }
public sealed class DeleteConsultaAttachmentUseCase(IConsultaRepository consultas) { public async Task ExecuteAsync(Guid attachmentId,CancellationToken ct=default) { var c=await consultas.FindByAttachmentIdAsync(attachmentId,ct)??throw new KeyNotFoundException(); var attachment=c.Attachments.Single(x=>x.Id==attachmentId); c.RemoveAttachment(attachment); await consultas.SaveChangesAsync(ct); } }
public sealed class GetConsultaAttachmentContentUseCase(IConsultaRepository consultas) { public async Task<(byte[] Content,string Type,string Name)?> ExecuteAsync(Guid attachmentId,CancellationToken ct=default) { var c=await consultas.FindByAttachmentIdAsync(attachmentId,ct); var a=c?.Attachments.SingleOrDefault(x=>x.Id==attachmentId); return a is null?null:(a.Contenido,a.TipoContenido,a.NombreArchivo); } }
public sealed class ExportConsultaPrescriptionPdfUseCase(IConsultaRepository consultas, GetPatientUseCase getPatient)
{
    public async Task<(byte[] File, string FileName)> ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var consulta = await consultas.FindByIdAsync(id, ct) ?? throw new ConsultaNotFoundException(id);
        var patient = await getPatient.ExecuteAsync(consulta.PatientId, ct) ?? throw new ArgumentException("El paciente indicado no existe.");
        var logoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "clinic-logo.png");
        var logo = File.Exists(logoPath) ? File.ReadAllBytes(logoPath) : null;
        var file = Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4); page.Margin(2, Unit.Centimetre);
                page.Header().Row(row => { if (logo is not null) row.ConstantItem(75).Height(45).Image(logo); row.RelativeItem().Column(column => { column.Item().Text("Dra. María Isabela Pavón — Ginecóloga y Obstetra").FontSize(14).SemiBold(); column.Item().Text("Receta médica").FontSize(11).FontColor(Colors.Grey.Darken1); }); });
                page.Content().PaddingVertical(15).Column(column =>
                {
                    column.Spacing(8); column.Item().Text($"Paciente: {patient.NombreCompleto}").SemiBold(); column.Item().Text($"Fecha: {consulta.Fecha:dd/MM/yyyy}"); column.Item().PaddingTop(8).Text("Medicamentos").FontSize(13).SemiBold();
                    if (consulta.PrescriptionItems.Count == 0) column.Item().Text("No se indicaron medicamentos.").Italic();
                    foreach (var item in consulta.PrescriptionItems.OrderBy(x => x.Orden)) column.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(5).Text(text => { text.Span($"{item.Medicamento}: ").SemiBold(); text.Span($"{item.Dosis}, {item.Via}, {item.Frecuencia}, {item.DuracionTexto}"); });
                });
            });
        }).GeneratePdf();
        return (file, SuggestedFileName(patient.NombreCompleto));
    }
    public static string SuggestedFileName(string name) { var normalized = name.Normalize(NormalizationForm.FormD); var builder = new StringBuilder(); foreach (var character in normalized) if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark && char.IsLetterOrDigit(character)) builder.Append(character); return $"receta-{builder}.pdf"; }
}
