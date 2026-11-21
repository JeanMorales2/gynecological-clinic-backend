using Ginecologia.Modules.Facturacion.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace Ginecologia.Modules.Facturacion.Infrastructure;
public static class FacturacionServiceCollectionExtensions
{ public static IServiceCollection AddFacturacionModule(this IServiceCollection services, string connectionString) { services.AddDbContext<FacturacionDbContext>(options => options.UseNpgsql(connectionString)); services.AddScoped<IInvoiceRepository, InvoiceRepository>(); services.AddScoped<InvoiceValidator>(); services.AddScoped<CreateInvoiceUseCase>(); services.AddScoped<MarkInvoicePaidUseCase>(); services.AddScoped<VoidInvoiceUseCase>(); services.AddScoped<GetInvoiceUseCase>(); services.AddScoped<ListInvoicesUseCase>(); return services; } }
