using FavoriteRates.FinanceService.Domain.Entities;

namespace FavoriteRates.FinanceService.Domain.Repositories;

public interface IUserFavoritesRepository
{
    Task UpdateAllAsync(UserFavorite[] userFavorites, CancellationToken cancellationToken);
}