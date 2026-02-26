using FavoriteRates.UsersService.Application.Abstractions;
using FavoriteRates.UsersService.Infrastructure.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace FavoriteRates.UsersService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        
        return services;
    }
}