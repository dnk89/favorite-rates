using FavoriteRates.FinanceService.Application.Currencies.SetFavorites;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FavoriteRates.FinanceService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromAssembliesOf(typeof(SetFavoritesCommandHandler))
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("CommandHandler")))
            .AsSelf()
            .WithTransientLifetime());
        
        services.AddValidatorsFromAssembly(typeof(SetFavoritesCommandValidator).Assembly);
        
        return services;   
    }
}