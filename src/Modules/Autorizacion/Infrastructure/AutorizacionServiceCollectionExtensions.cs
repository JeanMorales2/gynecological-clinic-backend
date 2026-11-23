using Ginecologia.Modules.Autorizacion.Api;
using Ginecologia.Modules.Autorizacion.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ginecologia.Modules.Autorizacion.Infrastructure;

public static class AutorizacionServiceCollectionExtensions
{
    public static IServiceCollection AddAutorizacionModule(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AutorizacionDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<ListPermissionsUseCase>();
        services.AddScoped<ListGrantedPermissionKeysUseCase>();
        services.AddScoped<SetPermissionGrantUseCase>();
        services.AddScoped<IsRolePermittedUseCase>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        return services;
    }
}
