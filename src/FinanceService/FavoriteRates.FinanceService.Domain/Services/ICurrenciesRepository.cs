using FavoriteRates.FinanceService.Domain.Entities;

namespace FavoriteRates.FinanceService.Domain.Services;

public interface ICurrenciesRepository
{
    Task<Currency?> FindByCodeAsync(string code, CancellationToken cancellationToken);
    
    Task AddAsync(Currency currency, CancellationToken cancellationToken);
    
    Task UpdateAsync(Currency currency, CancellationToken cancellationToken);
}