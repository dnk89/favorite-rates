using FavoriteRates.SharedLibrary.Application;
using FavoriteRates.SharedLibrary.ResultPattern;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FavoriteRates.UsersService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        
        services.Scan(scan => scan.FromAssemblies(assembly)
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Handler")))
            .AsImplementedInterfaces()
            .WithTransientLifetime());
        
        services.AddValidatorsFromAssembly(assembly);
        
        services.Decorate(typeof(IHandler<,>), typeof(ValidationDecorator<,>));
        
        return services;
    }   
}