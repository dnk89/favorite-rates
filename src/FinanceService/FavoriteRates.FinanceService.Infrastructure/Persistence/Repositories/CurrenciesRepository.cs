using FavoriteRates.FinanceService.Domain.Entities;
using FavoriteRates.FinanceService.Domain.Services;
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
}