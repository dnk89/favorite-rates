using FavoriteRates.SharedLibrary.Application;
using FavoriteRates.SharedLibrary.ResultPattern;
using FavoriteRates.UsersService.Application.Users.Login;
using FavoriteRates.UsersService.Application.Users.Register;

namespace FavoriteRates.UsersService.Endpoints;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users");

        group.MapPost("/register",
            async (RegisterUserCommand command, IHandler<RegisterUserCommand, Result<RegisteredUserDto>> handler, CancellationToken ct) =>
                (await handler.Handle(command, ct)).ToProblemDetails());

        group.MapPost("/login",
            async (LoginUserCommand command, IHandler<LoginUserCommand, Result<UserTokenDto>> handler, CancellationToken ct) => 
                (await handler.Handle(command, ct)).ToProblemDetails());
    }
}