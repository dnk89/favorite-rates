namespace FavoriteRates.FinanceService.Application.Abstractions;

public interface IUserContext
{
    Guid? GetCurrentUserId();
}