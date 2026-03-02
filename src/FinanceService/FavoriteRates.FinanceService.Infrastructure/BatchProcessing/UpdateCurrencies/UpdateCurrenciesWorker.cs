using FavoriteRates.FinanceService.Application.Abstractions;
using FavoriteRates.FinanceService.Application.Currencies.Update;
using FavoriteRates.FinanceService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FavoriteRates.FinanceService.Infrastructure.BatchProcessing.UpdateCurrencies;

public class UpdateCurrenciesWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<UpdateCurrenciesWorker> logger,
    IOptions<UpdateCurrenciesOptions> options,
    TimeProvider timeProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (options.Value.UpdateIfEmpty)
        {
            await RunAsync(stoppingToken, true);
        }
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = CalculateDelay(options.Value.UpdateAt, timeProvider.GetLocalNow());
            logger.LogInformation("Next update of CBR currencies will be in {delay}.", delay);
            
            await Task.Delay(delay, stoppingToken);
            
            await RunAsync(stoppingToken);
        }
    }

    private static TimeSpan CalculateDelay(TimeSpan updateAt, DateTimeOffset now)
    {
        var nextRunTime = now.Date.Add(updateAt);
        if (nextRunTime < now)
        {
            nextRunTime = nextRunTime.AddDays(1);
        }
        return nextRunTime - now;
    }
    
    private async Task RunAsync(CancellationToken stoppingToken, bool onlyEmpty = false)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IUpdateCurrenciesService>();
            if (onlyEmpty)
            {
                var db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
                if (db.Currencies.Any()) return;
            }
            await service.UpdateAsync(stoppingToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Update of CBR currencies failed.");
        }
    }
}