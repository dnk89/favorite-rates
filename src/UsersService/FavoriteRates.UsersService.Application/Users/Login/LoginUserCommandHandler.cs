using FavoriteRates.SharedLibrary.Application;
using FavoriteRates.SharedLibrary.ResultPattern;
using FavoriteRates.UsersService.Application.Abstractions;
using FavoriteRates.UsersService.Domain.Repositories;

namespace FavoriteRates.UsersService.Application.Users.Login;

public class LoginUserCommandHandler(
    IUsersRepository usersRepository,
    IPasswordHasher passwordHasher,
    IUserTokenProvider tokenProvider) : IHandler<LoginUserCommand, Result<UserTokenDto>>
{
    public async Task<Result<UserTokenDto>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var user = await usersRepository.FindByNameAsync(command.Name, cancellationToken);
        if (user is null)
        {
            return Result<UserTokenDto>.Failure("Login failed.");      
        }
        
        var validPassword = passwordHasher.Verify(command.Password, user.PasswordHash);
        if (!validPassword)
        {
            return Result<UserTokenDto>.Failure("Login failed.");
        }

        var token = tokenProvider.GenerateToken(user);
        
        return Result<UserTokenDto>.Success(new UserTokenDto(token));
    }
}