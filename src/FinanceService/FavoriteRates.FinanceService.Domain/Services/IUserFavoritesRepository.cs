using FavoriteRates.FinanceService.Domain.Entities;

namespace FavoriteRates.FinanceService.Domain.Services;

public interface IUserFavoritesRepository
{
    Task UpdateAllAsync(UserFavorite[] userFavorites, CancellationToken cancellationToken);
}