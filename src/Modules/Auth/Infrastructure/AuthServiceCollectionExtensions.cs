using Ginecologia.Modules.Auth.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ginecologia.Modules.Auth.Infrastructure;

public static class AuthServiceCollectionExtensions
{
    public static IServiceCollection AddAuthModule(this IServiceCollection services, string connectionString, JwtOptions jwtOptions, AuthFrontendOptions frontendOptions)
    {
        services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "auth")));
        services.AddScoped<IAuthAccountRepository, AuthAccountRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddSingleton<IPasswordService, PasswordHasherService>();
        services.AddSingleton(jwtOptions);
        services.AddSingleton(frontendOptions);
        services.AddSingleton<ITokenIssuer, JwtTokenIssuer>();
        services.AddScoped<IAuthAccountManager, AuthAccountManager>();
        services.AddScoped<LoginUseCase>();
        services.AddScoped<GetCurrentUserUseCase>();
        services.AddScoped<ChangePasswordUseCase>();
        services.AddScoped<RequestPasswordResetUseCase>();
        services.AddScoped<ResetPasswordUseCase>();
        return services;
    }
}
