using FavoriteRates.FinanceService.Application.Currencies.Update;
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
    public const int RetryDelayOnExactMatchSeconds = 5;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (options.Value.UpdateOnStart)
        {
            await RunAsync(stoppingToken);
        }
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = CalculateDelay(options.Value.UpdateAt, timeProvider.GetLocalNow());
            logger.LogInformation("Next update of CBR currencies will be in {delay}.", delay);
            
            await Task.Delay(delay, stoppingToken);
            
            await RunAsync(stoppingToken);
        }
    }

    public static TimeSpan CalculateDelay(TimeSpan updateAt, DateTimeOffset now)
    {
        var nextRunTime = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, now.Offset).Add(updateAt);
        
        if (nextRunTime == now)
        {
            return TimeSpan.FromSeconds(RetryDelayOnExactMatchSeconds);
        }
        
        if (nextRunTime < now)
        {
            nextRunTime = nextRunTime.AddDays(1);
        }
        return nextRunTime - now;
    }
    
    private async Task RunAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IUpdateCurrenciesService>();
            await service.UpdateAsync(stoppingToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Update of CBR currencies failed.");
        }
    }
}
