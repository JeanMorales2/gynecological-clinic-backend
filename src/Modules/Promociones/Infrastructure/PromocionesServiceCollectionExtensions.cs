using Ginecologia.Modules.Promociones.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace Ginecologia.Modules.Promociones.Infrastructure;
public static class PromocionesServiceCollectionExtensions
{ public static IServiceCollection AddPromocionesModule(this IServiceCollection services, string connectionString) { services.AddDbContext<PromocionesDbContext>(x => x.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "promociones"))); services.AddScoped<IPromotionRepository, PromotionRepository>(); services.AddScoped<GenerateBirthdayPromotionsUseCase>(); services.AddScoped<CreatePromotionUseCase>(); services.AddScoped<DiscardPromotionUseCase>(); services.AddScoped<DeletePromotionUseCase>(); services.AddScoped<GetPromotionUseCase>(); services.AddScoped<ListPromotionsUseCase>(); services.AddHostedService<BirthdayPromotionBackgroundService>(); return services; } }
