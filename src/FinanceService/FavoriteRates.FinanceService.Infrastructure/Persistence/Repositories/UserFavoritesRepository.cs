using FavoriteRates.FinanceService.Domain.Entities;
using FavoriteRates.FinanceService.Domain.Repositories;

namespace FavoriteRates.FinanceService.Infrastructure.Persistence.Repositories;

public class UserFavoritesRepository(FinanceDbContext dbContext) : IUserFavoritesRepository
{
    public async Task UpdateAllAsync(UserFavorite[] userFavorites, CancellationToken cancellationToken)
    {
        var userIds = userFavorites.Select(x => x.UserId).Distinct();
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        dbContext.UserFavorites.RemoveRange(dbContext.UserFavorites.Where(x => userIds.Contains(x.UserId)));
        await dbContext.SaveChangesAsync(cancellationToken);
        await dbContext.UserFavorites.AddRangeAsync(userFavorites);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}