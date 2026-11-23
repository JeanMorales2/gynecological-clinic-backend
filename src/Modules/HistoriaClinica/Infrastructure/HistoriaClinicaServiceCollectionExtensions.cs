using Ginecologia.Modules.HistoriaClinica.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ginecologia.Modules.HistoriaClinica.Infrastructure;

public static class HistoriaClinicaServiceCollectionExtensions
{
    public static IServiceCollection AddHistoriaClinicaModule(this IServiceCollection services, string connectionString)
    {
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        services.AddDbContext<HistoriaClinicaDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IClinicalEntryRepository, ClinicalEntryRepository>();
        services.AddScoped<ClinicalEntryPatientValidator>();
        services.AddScoped<CreateClinicalEntryUseCase>();
        services.AddScoped<UpdateClinicalEntryUseCase>();
        services.AddScoped<GetClinicalEntryUseCase>();
        services.AddScoped<ListClinicalEntriesUseCase>();
        services.AddScoped<ExportClinicalHistoryPdfUseCase>();
        return services;
    }
}
