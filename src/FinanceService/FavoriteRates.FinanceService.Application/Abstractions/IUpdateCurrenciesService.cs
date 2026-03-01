namespace FavoriteRates.FinanceService.Application.Abstractions;

public interface IUpdateCurrenciesService
{
    Task UpdateAsync(CancellationToken cancellationToken);
}