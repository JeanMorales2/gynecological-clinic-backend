using Ginecologia.Modules.Auth.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ginecologia.Modules.Auth.Infrastructure;

public static class AuthServiceCollectionExtensions
{
    public static IServiceCollection AddAuthModule(this IServiceCollection services, string connectionString, JwtOptions jwtOptions)
    {
        services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IAuthAccountRepository, AuthAccountRepository>();
        services.AddSingleton<IPasswordService, PasswordHasherService>();
        services.AddSingleton(jwtOptions);
        services.AddSingleton<ITokenIssuer, JwtTokenIssuer>();
        services.AddScoped<IAuthAccountManager, AuthAccountManager>();
        services.AddScoped<LoginUseCase>();
        services.AddScoped<GetCurrentUserUseCase>();
        return services;
    }
}
