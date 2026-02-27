using FavoriteRates.UsersService.Domain.Entities;

namespace FavoriteRates.UsersService.Domain.Services;

public interface IUsersRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken);
    
    Task<bool> ExistsWithNameAsync(string name, CancellationToken cancellationToken);
}