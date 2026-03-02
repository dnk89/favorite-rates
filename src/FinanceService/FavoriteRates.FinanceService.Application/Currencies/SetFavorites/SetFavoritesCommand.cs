namespace FavoriteRates.FinanceService.Application.Currencies.SetFavorites;

public sealed record SetFavoritesCommand(IEnumerable<string> Currencies);