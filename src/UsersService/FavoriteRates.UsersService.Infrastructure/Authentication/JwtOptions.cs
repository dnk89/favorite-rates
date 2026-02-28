using System.ComponentModel.DataAnnotations;

namespace FavoriteRates.UsersService.Infrastructure.Authentication;

public class JwtOptions
{
    public const string Key = "Jwt";

    [Required]
    public string Secret { get; set; } = null!;

    [Range(1, int.MaxValue)]
    public int ExpirationInMinutes { get; set; } = TimeSpan.FromMinutes(30).Minutes;

    [Required]
    public string Issuer { get; set; } = null!;

    [Required]
    public string Audience { get; set; } = null!;
}