namespace FavoriteRates.UsersService.Application.Users.Register;

public sealed record RegisterUserCommand(string Name, string Password1, string Password2);