using Ginecologia.Modules.Promociones.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace Ginecologia.Modules.Promociones.Infrastructure;
public static class PromocionesServiceCollectionExtensions
{ public static IServiceCollection AddPromocionesModule(this IServiceCollection services, string connectionString) { services.AddDbContext<PromocionesDbContext>(x => x.UseNpgsql(connectionString)); services.AddScoped<IPromotionRepository, PromotionRepository>(); services.AddScoped<GenerateBirthdayPromotionsUseCase>(); services.AddScoped<DiscardPromotionUseCase>(); services.AddScoped<GetPromotionUseCase>(); services.AddScoped<ListPromotionsUseCase>(); services.AddHostedService<BirthdayPromotionBackgroundService>(); return services; } }
