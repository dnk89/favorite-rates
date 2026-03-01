using FavoriteRates.SharedLibrary.ResultPattern;
using FavoriteRates.UsersService.Application.Abstractions;
using FavoriteRates.UsersService.Application.Dtos;
using FavoriteRates.UsersService.Domain.Services;
using FluentValidation;

namespace FavoriteRates.UsersService.Application.Users.Login;

public class LoginUserCommandHandler(
    IValidator<LoginUserCommand> validator,
    IUsersRepository usersRepository,
    IPasswordHasher passwordHasher,
    IUserTokenProvider tokenProvider)
{
    public async Task<Result<TokenDto>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            var error = validation.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed.";
            return Result<TokenDto>.Failure(error);       
        }
        
        var user = await usersRepository.FindByNameAsync(command.Name, cancellationToken);
        if (user is null)
        {
            return Result<TokenDto>.Failure("Login failed.");      
        }
        
        var validPassword = passwordHasher.Verify(command.Password, user.PasswordHash);
        if (!validPassword)
        {
            return Result<TokenDto>.Failure("Login failed.");
        }

        var token = tokenProvider.GenerateToken(user);
        
        return Result<TokenDto>.Success(new TokenDto(token));
    }
}