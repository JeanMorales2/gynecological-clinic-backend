using Ginecologia.Modules.LaboratorioExterno.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace Ginecologia.Modules.LaboratorioExterno.Infrastructure;
public static class LaboratorioExternoServiceCollectionExtensions
{ public static IServiceCollection AddLaboratorioExternoModule(this IServiceCollection services, string connectionString) { services.AddDbContext<LaboratorioExternoDbContext>(options => options.UseNpgsql(connectionString)); services.AddScoped<ILabOrderRepository, LabOrderRepository>(); services.AddScoped<ILabOrderAttachmentRepository, LabOrderAttachmentRepository>(); services.AddScoped<LabOrderPatientValidator>(); services.AddScoped<CreateLabOrderUseCase>(); services.AddScoped<MarkLabOrderReceivedUseCase>(); services.AddScoped<CancelLabOrderUseCase>(); services.AddScoped<GetLabOrderUseCase>(); services.AddScoped<ListLabOrdersUseCase>(); services.AddScoped<UploadLabOrderAttachmentUseCase>(); services.AddScoped<ListLabOrderAttachmentsUseCase>(); services.AddScoped<DeleteLabOrderAttachmentUseCase>(); services.AddScoped<GetLabOrderAttachmentContentUseCase>(); return services; } }
