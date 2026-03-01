namespace FavoriteRates.FinanceService.Domain.Entities;

public class Currency
{
    public const int IdLength = 3;
    public const int NameLength = 50;
    
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
}