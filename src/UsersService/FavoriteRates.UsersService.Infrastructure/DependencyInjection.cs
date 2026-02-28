using FavoriteRates.UsersService.Application.Abstractions;
using FavoriteRates.UsersService.Domain.Services;
using FavoriteRates.UsersService.Infrastructure.Authentication;
using FavoriteRates.UsersService.Infrastructure.Persistence;
using FavoriteRates.UsersService.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FavoriteRates.UsersService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IUserTokenProvider, UserJwtTokenProvider>();
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.Key))
            .ValidateDataAnnotations();

        services.AddScoped<UsersDbContext>(sp => new UsersDbContext(
            configuration.GetConnectionString("DefaultConnection")!,
            sp.GetRequiredService<ILoggerFactory>()));
        
        services.AddTransient<IUsersRepository, UsersRepository>();
        
        return services;
    }
}