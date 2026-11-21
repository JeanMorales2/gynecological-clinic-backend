using Ginecologia.Modules.Pacientes.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ginecologia.Modules.Pacientes.Infrastructure;

public static class PacientesServiceCollectionExtensions
{
    public static IServiceCollection AddPacientesModule(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<PacientesDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<CreatePatientUseCase>();
        services.AddScoped<UpdatePatientUseCase>();
        services.AddScoped<DeactivatePatientUseCase>();
        services.AddScoped<GetPatientUseCase>();
        services.AddScoped<ListPatientsUseCase>();
        services.AddScoped<ListActivePatientsBirthdaysUseCase>();
        return services;
    }
}
