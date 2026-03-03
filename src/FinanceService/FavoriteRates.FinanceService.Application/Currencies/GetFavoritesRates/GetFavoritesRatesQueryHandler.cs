using FavoriteRates.FinanceService.Application.Abstractions;
using FavoriteRates.FinanceService.Domain.Repositories;
using FavoriteRates.SharedLibrary.Application;
using FavoriteRates.SharedLibrary.ResultPattern;

namespace FavoriteRates.FinanceService.Application.Currencies.GetFavoritesRates;

public class GetFavoritesRatesQueryHandler(
    ICurrenciesRepository currenciesRepository,
    IUserContext userContext) : IHandler<GetFavoritesRatesQuery, Result<IEnumerable<CurrencyRateDto>>>
{
    public async Task<Result<IEnumerable<CurrencyRateDto>>> Handle(
        GetFavoritesRatesQuery query,
        CancellationToken cancellationToken)
    {
        var currentUserId = userContext.GetCurrentUserId();
        if (currentUserId is null)
            return Result<IEnumerable<CurrencyRateDto>>.Failure("User not authenticated.");
        
        var rates = (await currenciesRepository.GetFavoritesAsync(currentUserId.Value, cancellationToken))
            .Select(c => new CurrencyRateDto(c.Id, c.Name, c.Rate))
            .ToList();
        
        return Result<IEnumerable<CurrencyRateDto>>.Success(rates);
    }
}