using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FavoriteRates.UsersService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromAssembliesOf(typeof(ApplicationAssembly))
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("CommandHandler")))
            .AsSelf()
            .WithTransientLifetime());
        
        services.AddValidatorsFromAssembly(typeof(ApplicationAssembly).Assembly);
        
        return services;
    }   
}