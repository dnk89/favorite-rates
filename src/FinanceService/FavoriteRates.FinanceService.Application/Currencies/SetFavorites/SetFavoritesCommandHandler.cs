using FavoriteRates.FinanceService.Application.Abstractions;
using FavoriteRates.FinanceService.Domain.Entities;
using FavoriteRates.FinanceService.Domain.Repositories;
using FavoriteRates.SharedLibrary.Application;
using FavoriteRates.SharedLibrary.ResultPattern;

namespace FavoriteRates.FinanceService.Application.Currencies.SetFavorites;

public class SetFavoritesCommandHandler(
    IUserContext userContext,
    IUserFavoritesRepository userFavoritesRepository) : IHandler<SetFavoritesCommand, Result>
{
    public async Task<Result> Handle(SetFavoritesCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = userContext.GetCurrentUserId();
        if (currentUserId is null)
            return Result.Failure("User not authenticated.");
        
        var userFavorites = command.Currencies
            .Select(c => new UserFavorite
            {
                UserId = currentUserId.Value,
                CurrencyId = c.ToUpper()
            })
            .ToArray();
        await userFavoritesRepository.UpdateAllAsync(userFavorites, cancellationToken);
        
        return Result.Success();
    }
}