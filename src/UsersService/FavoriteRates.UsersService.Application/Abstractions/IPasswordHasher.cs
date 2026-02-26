namespace FavoriteRates.UsersService.Application.Abstractions;

public interface IPasswordHasher
{
    string Hash(string password);
}