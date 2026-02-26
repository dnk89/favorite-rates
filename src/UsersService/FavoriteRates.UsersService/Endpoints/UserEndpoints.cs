using FavoriteRates.UsersService.Application.Users.Commands.RegisterUser;
using FavoriteRates.UsersService.Extensions;

namespace FavoriteRates.UsersService.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users");

        group.MapPost("/register",
            async (RegisterUserCommand command, RegisterUserCommandHandler commandHandler, CancellationToken ct) =>
                (await commandHandler.Handle(command, ct)).ToProblemDetails());
    }
}