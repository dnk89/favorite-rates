namespace FavoriteRates.UsersService.Domain.Entities;

public class User
{
    public const int MinNameLength = 6;
    public const int MaxNameLength = 50;
    public const int MinPasswordLength = 8;
    
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
}