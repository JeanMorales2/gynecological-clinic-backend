using Ginecologia.Modules.Facturacion.Domain;
using Microsoft.EntityFrameworkCore;
namespace Ginecologia.Modules.Facturacion.Infrastructure;
public sealed class FacturacionDbContext(DbContextOptions<FacturacionDbContext> options) : DbContext(options)
{
    public DbSet<Invoice> Invoices => Set<Invoice>(); public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("facturacion"); var invoice = modelBuilder.Entity<Invoice>(); invoice.ToTable("invoices"); invoice.HasKey(x => x.Id); invoice.Property(x => x.Id).HasColumnName("id"); invoice.Property(x => x.PatientId).HasColumnName("patient_id").IsRequired(); invoice.Property(x => x.AppointmentId).HasColumnName("appointment_id"); invoice.Property(x => x.NumeroFactura).HasColumnName("numero_factura").HasMaxLength(30).IsRequired(); invoice.HasIndex(x => x.NumeroFactura).IsUnique(); invoice.Property(x => x.Fecha).HasColumnName("fecha").HasColumnType("date").IsRequired(); invoice.Property(x => x.MontoTotal).HasColumnName("monto_total").HasPrecision(18, 2).IsRequired(); invoice.Property(x => x.Estado).HasColumnName("estado").HasConversion<string>().HasMaxLength(20).IsRequired(); invoice.Property(x => x.MetodoPago).HasColumnName("metodo_pago").HasMaxLength(100); invoice.Property(x => x.FechaPago).HasColumnName("fecha_pago").HasColumnType("date"); invoice.Property(x => x.CreadoPorId).HasColumnName("creado_por_id").IsRequired(); invoice.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Cascade);
        var item = modelBuilder.Entity<InvoiceItem>(); item.ToTable("invoice_items"); item.HasKey(x => x.Id); item.Property(x => x.Id).HasColumnName("id"); item.Property(x => x.InvoiceId).HasColumnName("invoice_id").IsRequired(); item.Property(x => x.Concepto).HasColumnName("concepto").HasMaxLength(300).IsRequired(); item.Property(x => x.Cantidad).HasColumnName("cantidad").IsRequired(); item.Property(x => x.PrecioUnitario).HasColumnName("precio_unitario").HasPrecision(18, 2).IsRequired(); item.Property(x => x.Orden).HasColumnName("orden").IsRequired(); invoice.HasIndex(x => new { x.PatientId, x.Fecha }).HasDatabaseName("ix_invoices_patient_fecha");
    }
}
