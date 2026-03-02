using FavoriteRates.FinanceService.Application.Currencies.SetFavorites;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FavoriteRates.FinanceService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(SetFavoritesCommandHandler).Assembly;
        
        services.Scan(scan => scan.FromAssemblies(assembly)
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("CommandHandler")))
            .AsSelf()
            .WithTransientLifetime());
        services.Scan(scan => scan.FromAssemblies(assembly)
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("QueryHandler")))
            .AsSelf()
            .WithTransientLifetime());
        
        services.AddValidatorsFromAssembly(assembly);
        
        return services;   
    }
}