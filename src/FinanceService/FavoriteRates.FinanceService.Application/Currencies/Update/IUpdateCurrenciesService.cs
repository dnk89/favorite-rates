namespace FavoriteRates.FinanceService.Application.Currencies.Update;

public interface IUpdateCurrenciesService
{
    Task UpdateAsync(CancellationToken cancellationToken);
}