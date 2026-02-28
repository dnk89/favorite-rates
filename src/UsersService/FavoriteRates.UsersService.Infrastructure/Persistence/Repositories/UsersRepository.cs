using FavoriteRates.UsersService.Domain.Entities;
using FavoriteRates.UsersService.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace FavoriteRates.UsersService.Infrastructure.Persistence.Repositories;

public class UsersRepository(UsersDbContext db) : IUsersRepository
{
    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await db.Users.AddAsync(user, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsWithNameAsync(string name, CancellationToken cancellationToken)
    {
        return await db.Users
            .FirstOrDefaultAsync(u => name.ToLower() == u.Name.ToLower(), cancellationToken) is not null;
    }

    public async Task<User?> FindByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await db.Users.SingleOrDefaultAsync(u => name.ToLower() == u.Name.ToLower(), cancellationToken);
    }
}