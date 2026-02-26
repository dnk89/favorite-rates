using FavoriteRates.UsersService.Domain.Entities;

namespace FavoriteRates.UsersService.Domain.Services;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken);
    
    Task<bool> ExistsWithNameAsync(string name, CancellationToken cancellationToken);
}