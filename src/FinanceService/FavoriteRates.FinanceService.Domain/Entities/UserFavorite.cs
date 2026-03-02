namespace FavoriteRates.FinanceService.Domain.Entities;

public class UserFavorite
{
    public Guid UserId { get; set; }
    
    public string CurrencyId { get; set; } = string.Empty;

    public Currency Currency { get; set; } = null!;
}