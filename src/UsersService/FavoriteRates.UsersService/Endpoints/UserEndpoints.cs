using FavoriteRates.SharedLibrary.ResultPattern;
using FavoriteRates.UsersService.Application.Users.Login;
using FavoriteRates.UsersService.Application.Users.Register;

namespace FavoriteRates.UsersService.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users");

        group.MapPost("/register",
            async (RegisterUserCommand command, RegisterUserCommandHandler commandHandler, CancellationToken ct) =>
                (await commandHandler.Handle(command, ct)).ToProblemDetails());

        group.MapPost("/login",
            async (LoginUserCommand command, LoginUserCommandHandler  commandHandler, CancellationToken ct) => 
                (await commandHandler.Handle(command, ct)).ToProblemDetails());
    }
}