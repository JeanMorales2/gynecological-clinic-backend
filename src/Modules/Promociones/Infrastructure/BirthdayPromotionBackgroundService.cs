using Ginecologia.Modules.Promociones.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace Ginecologia.Modules.Promociones.Infrastructure;
public sealed class BirthdayPromotionBackgroundService(IServiceScopeFactory scopeFactory, ILogger<BirthdayPromotionBackgroundService> logger) : BackgroundService
{ protected override async Task ExecuteAsync(CancellationToken stoppingToken) { while (!stoppingToken.IsCancellationRequested) { var now = DateTime.Now; var nextMidnight = now.Date.AddDays(1); await Task.Delay(nextMidnight - now, stoppingToken); try { using var scope = scopeFactory.CreateScope(); var useCase = scope.ServiceProvider.GetRequiredService<GenerateBirthdayPromotionsUseCase>(); var generated = await useCase.ExecuteAsync(DateOnly.FromDateTime(DateTime.UtcNow), stoppingToken); logger.LogInformation("Se generaron {Count} promociones de cumpleaños.", generated); } catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { } catch (Exception ex) { logger.LogError(ex, "Error al generar promociones de cumpleaños."); } } } }
