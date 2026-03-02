using FavoriteRates.FinanceService.Application.Abstractions;
using FavoriteRates.FinanceService.Domain.Entities;
using FavoriteRates.FinanceService.Domain.Repositories;
using FavoriteRates.SharedLibrary.ResultPattern;
using FluentValidation;

namespace FavoriteRates.FinanceService.Application.Currencies.SetFavorites;

public class SetFavoritesCommandHandler(
    IValidator<SetFavoritesCommand> validator,
    IUserContext userContext,
    IUserFavoritesRepository userFavoritesRepository)
{
    public async Task<Result> Handle(SetFavoritesCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            var error = validation.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed.";
            return Result.Failure(error);
        }

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