using System.ComponentModel.DataAnnotations;

namespace FavoriteRates.FinanceService.Infrastructure.BatchProcessing.UpdateCurrencies;

public class UpdateCurrenciesOptions
{
    public const string Key = "UpdateCurrencies";
    
    [Required]
    public TimeSpan UpdateAtUtc { get; set; } = TimeSpan.Parse("00:00:00");
    
    public bool UpdateOnStart { get; set; } = false;
    
    [Required]
    public string ClientBaseUrl { get; set; } = string.Empty;
    
    [Required]
    public string ClientPath { get; set; } = string.Empty;
}