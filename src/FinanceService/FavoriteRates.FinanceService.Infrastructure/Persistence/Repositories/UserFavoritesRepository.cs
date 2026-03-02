using FavoriteRates.FinanceService.Domain.Entities;
using FavoriteRates.FinanceService.Domain.Services;

namespace FavoriteRates.FinanceService.Infrastructure.Persistence.Repositories;

public class UserFavoritesRepository(FinanceDbContext dbContext) : IUserFavoritesRepository
{
    public async Task UpdateAllAsync(UserFavorite[] userFavorites, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        dbContext.UserFavorites.RemoveRange(userFavorites);
        await dbContext.UserFavorites.AddRangeAsync(userFavorites);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}