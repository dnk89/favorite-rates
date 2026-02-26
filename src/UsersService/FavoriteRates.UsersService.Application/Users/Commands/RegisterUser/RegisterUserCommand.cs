namespace FavoriteRates.UsersService.Application.Users.Commands.RegisterUser;

public sealed record RegisterUserCommand(string Name, string Password1, string Password2);