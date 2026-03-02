using FavoriteRates.FinanceService.Domain.Entities;
using FavoriteRates.FinanceService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FavoriteRates.FinanceService.Infrastructure.Persistence.Repositories;

public class CurrenciesRepository(FinanceDbContext dbContext) : ICurrenciesRepository
{
    public async Task<Currency?> FindByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await dbContext.Currencies.SingleOrDefaultAsync(c => c.Id == code, cancellationToken);
    }

    public async Task AddAsync(Currency currency, CancellationToken cancellationToken)
    {
        await dbContext.Currencies.AddAsync(currency, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task UpdateAsync(Currency currency, CancellationToken cancellationToken)
    {
        dbContext.Currencies.Update(currency);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<Currency>> GetFavoritesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.UserFavorites
            .Include(uf => uf.Currency)
            .AsNoTracking()
            .Where(uf => uf.UserId == userId)
            .Select(uf => uf.Currency)
            .ToListAsync(cancellationToken);
    }   
}