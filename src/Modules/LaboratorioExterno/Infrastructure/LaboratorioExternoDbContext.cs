using Ginecologia.Modules.LaboratorioExterno.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.LaboratorioExterno.Infrastructure;

public sealed class LaboratorioExternoDbContext(DbContextOptions<LaboratorioExternoDbContext> options) : DbContext(options)
{
    public DbSet<LabOrder> LabOrders => Set<LabOrder>();
    public DbSet<LabOrderAttachment> LabOrderAttachments => Set<LabOrderAttachment>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("laboratorio_externo"); var order = modelBuilder.Entity<LabOrder>(); order.ToTable("lab_orders"); order.HasKey(x => x.Id);
        order.Property(x => x.Id).HasColumnName("id"); order.Property(x => x.PatientId).HasColumnName("patient_id").IsRequired(); order.Property(x => x.TipoEstudio).HasColumnName("tipo_estudio").HasMaxLength(300).IsRequired(); order.Property(x => x.LaboratorioNombre).HasColumnName("laboratorio_nombre").HasMaxLength(300).IsRequired(); order.Property(x => x.FechaSolicitud).HasColumnName("fecha_solicitud").HasColumnType("date").IsRequired(); order.Property(x => x.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20).IsRequired(); order.Property(x => x.ResultadoResumen).HasColumnName("resultado_resumen").HasMaxLength(8000); order.Property(x => x.FechaResultado).HasColumnName("fecha_resultado").HasColumnType("date"); order.Property(x => x.Observaciones).HasColumnName("observaciones").HasMaxLength(4000); order.Property(x => x.SolicitadoPorId).HasColumnName("solicitado_por_id").IsRequired(); order.HasIndex(x => new { x.PatientId, x.FechaSolicitud }).HasDatabaseName("ix_lab_orders_patient_fecha");
        var attachment = modelBuilder.Entity<LabOrderAttachment>(); attachment.ToTable("lab_order_attachments"); attachment.HasKey(x => x.Id); attachment.Property(x => x.Id).HasColumnName("id"); attachment.Property(x => x.LabOrderId).HasColumnName("lab_order_id").IsRequired(); attachment.Property(x => x.NombreArchivo).HasColumnName("nombre_archivo").HasMaxLength(260).IsRequired(); attachment.Property(x => x.ContentType).HasColumnName("content_type").HasMaxLength(100).IsRequired(); attachment.Property(x => x.Contenido).HasColumnName("contenido").HasColumnType("bytea").IsRequired(); attachment.Property(x => x.TamanoBytes).HasColumnName("tamano_bytes").IsRequired(); attachment.Property(x => x.SubidoPorId).HasColumnName("subido_por_id").IsRequired(); attachment.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamp with time zone").IsRequired(); attachment.HasOne<LabOrder>().WithMany().HasForeignKey(x => x.LabOrderId).OnDelete(DeleteBehavior.Cascade); attachment.HasIndex(x => x.LabOrderId).HasDatabaseName("ix_lab_order_attachments_lab_order_id");
    }
}
