namespace FavoriteRates.FinanceService.Application.Currencies.GetFavoritesRates;

public sealed record CurrencyRateDto(string Code, string Name, decimal Rate);