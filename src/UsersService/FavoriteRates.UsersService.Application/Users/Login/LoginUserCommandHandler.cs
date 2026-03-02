using FavoriteRates.SharedLibrary.ResultPattern;
using FavoriteRates.UsersService.Application.Abstractions;
using FavoriteRates.UsersService.Domain.Repositories;
using FluentValidation;

namespace FavoriteRates.UsersService.Application.Users.Login;

public class LoginUserCommandHandler(
    IValidator<LoginUserCommand> validator,
    IUsersRepository usersRepository,
    IPasswordHasher passwordHasher,
    IUserTokenProvider tokenProvider)
{
    public async Task<Result<UserTokenDto>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            var error = validation.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed.";
            return Result<UserTokenDto>.Failure(error);       
        }
        
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