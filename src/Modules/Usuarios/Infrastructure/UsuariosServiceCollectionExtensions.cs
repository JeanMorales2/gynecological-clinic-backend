using Ginecologia.Modules.Usuarios.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ginecologia.Modules.Usuarios.Infrastructure;

public static class UsuariosServiceCollectionExtensions
{
    public static IServiceCollection AddUsuariosModule(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<UsuariosDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IStaffProfileRepository, StaffProfileRepository>();
        services.AddScoped<CreateStaffUseCase>();
        services.AddScoped<UpdateStaffUseCase>();
        services.AddScoped<DeactivateStaffUseCase>();
        services.AddScoped<ReactivateStaffUseCase>();
        services.AddScoped<ListStaffUseCase>();
        services.AddScoped<GetStaffUseCase>();
        return services;
    }
}
