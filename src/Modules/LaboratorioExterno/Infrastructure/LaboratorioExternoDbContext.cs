using Ginecologia.Modules.LaboratorioExterno.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ginecologia.Modules.LaboratorioExterno.Infrastructure;

public sealed class LaboratorioExternoDbContext(DbContextOptions<LaboratorioExternoDbContext> options) : DbContext(options)
{
    public DbSet<LabOrder> LabOrders => Set<LabOrder>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("laboratorio_externo"); var order = modelBuilder.Entity<LabOrder>(); order.ToTable("lab_orders"); order.HasKey(x => x.Id);
        order.Property(x => x.Id).HasColumnName("id"); order.Property(x => x.PatientId).HasColumnName("patient_id").IsRequired(); order.Property(x => x.TipoEstudio).HasColumnName("tipo_estudio").HasMaxLength(300).IsRequired(); order.Property(x => x.LaboratorioNombre).HasColumnName("laboratorio_nombre").HasMaxLength(300).IsRequired(); order.Property(x => x.FechaSolicitud).HasColumnName("fecha_solicitud").HasColumnType("date").IsRequired(); order.Property(x => x.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20).IsRequired(); order.Property(x => x.ResultadoResumen).HasColumnName("resultado_resumen").HasMaxLength(8000); order.Property(x => x.FechaResultado).HasColumnName("fecha_resultado").HasColumnType("date"); order.Property(x => x.Observaciones).HasColumnName("observaciones").HasMaxLength(4000); order.Property(x => x.SolicitadoPorId).HasColumnName("solicitado_por_id").IsRequired(); order.HasIndex(x => new { x.PatientId, x.FechaSolicitud }).HasDatabaseName("ix_lab_orders_patient_fecha");
    }
}
