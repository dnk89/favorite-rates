using FavoriteRates.FinanceService.Infrastructure.BatchProcessing.UpdateCurrencies;

namespace FavoriteRates.FinanceService.UnitTests.Infrastructure;

public class UpdateCurrenciesWorkerTests
{
    [Fact]
    public void CalculateDelay_BeforeScheduledTime_ReturnsDelayUntilScheduledTime()
    {
        var updateAt = new TimeSpan(10, 0, 0); // 10:00 AM
        var now = new DateTimeOffset(2026, 3, 2, 8, 0, 0, TimeSpan.Zero); // 8:00 AM
        var expected = TimeSpan.FromHours(2);

        var result = UpdateCurrenciesWorker.CalculateDelay(updateAt, now);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateDelay_AfterScheduledTime_ReturnsDelayUntilNextDayScheduledTime()
    {
        var updateAt = new TimeSpan(10, 0, 0); // 10:00 AM
        var now = new DateTimeOffset(2026, 3, 2, 11, 0, 0, TimeSpan.Zero); // 11:00 AM
        var expected = TimeSpan.FromHours(23); // Next day at 10 AM

        var result = UpdateCurrenciesWorker.CalculateDelay(updateAt, now);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateDelay_ExactlyAtScheduledTime_ReturnsRetryDelay()
    {
        var updateAt = new TimeSpan(10, 0, 0); // 10:00 AM
        var now = new DateTimeOffset(2026, 3, 2, 10, 0, 0, TimeSpan.Zero); // 10:00 AM
        var expected = TimeSpan.FromSeconds(UpdateCurrenciesWorker.RetryDelayOnExactMatchSeconds);

        var result = UpdateCurrenciesWorker.CalculateDelay(updateAt, now);

        Assert.Equal(expected, result);
    }
}
