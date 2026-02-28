using FavoriteRates.UsersService.Domain.Entities;

namespace FavoriteRates.UsersService.Application.Abstractions;

public interface IUserTokenProvider
{
    string GenerateToken(User user);
}