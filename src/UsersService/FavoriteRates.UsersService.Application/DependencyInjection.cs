using FavoriteRates.UsersService.Application.Users.Register;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FavoriteRates.UsersService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(RegisterUserCommandHandler).Assembly;
        
        services.Scan(scan => scan.FromAssemblies(assembly)
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("CommandHandler")))
            .AsSelf()
            .WithTransientLifetime());
        
        services.AddValidatorsFromAssembly(assembly);
        
        return services;
    }   
}