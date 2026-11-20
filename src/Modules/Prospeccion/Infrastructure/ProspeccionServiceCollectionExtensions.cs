using Ginecologia.Modules.Prospeccion.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ginecologia.Modules.Prospeccion.Infrastructure;

public static class ProspeccionServiceCollectionExtensions
{
    public static IServiceCollection AddProspeccionModule(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ProspeccionDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IProspectRepository, ProspectRepository>();
        services.AddScoped<CreateProspectUseCase>();
        services.AddScoped<UpdateProspectUseCase>();
        services.AddScoped<GetProspectUseCase>();
        services.AddScoped<ListProspectsUseCase>();
        services.AddScoped<ConvertProspectUseCase>();
        return services;
    }
}
