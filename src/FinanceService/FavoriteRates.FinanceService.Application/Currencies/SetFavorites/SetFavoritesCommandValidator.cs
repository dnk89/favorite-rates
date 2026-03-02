using FavoriteRates.FinanceService.Domain.Entities;
using FluentValidation;

namespace FavoriteRates.FinanceService.Application.Currencies.SetFavorites;

public class SetFavoritesCommandValidator : AbstractValidator<SetFavoritesCommand>
{
    public SetFavoritesCommandValidator()
    {
        RuleFor(x => x.Currencies)
            .NotEmpty().WithMessage("Currencies are required.");
        RuleForEach(x => x.Currencies)
            .Length(Currency.IdLength).WithMessage($"Currency must be {Currency.IdLength} characters long.")
            .NotEmpty().WithMessage("Currency is required.");       
    }
}