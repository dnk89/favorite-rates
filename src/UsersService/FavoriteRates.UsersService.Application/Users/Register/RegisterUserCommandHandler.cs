using FavoriteRates.SharedLibrary.ResultPattern;
using FavoriteRates.UsersService.Application.Abstractions;
using FavoriteRates.UsersService.Application.Dtos;
using FavoriteRates.UsersService.Domain.Entities;
using FavoriteRates.UsersService.Domain.Services;
using FluentValidation;

namespace FavoriteRates.UsersService.Application.Users.Register;

public sealed class RegisterUserCommandHandler(
    IValidator<RegisterUserCommand> validator,
    IPasswordHasher passwordHasher,
    IUsersRepository usersRepository)
{
    public async Task<Result<UserDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            var error = validation.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed.";
            return Result<UserDto>.Failure(error);
        }
        
        if (await usersRepository.ExistsWithNameAsync(request.Name, cancellationToken))
        {
            return Result<UserDto>.Failure("User with this name already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name.ToLower(),
            PasswordHash = passwordHasher.Hash(request.Password1)
        };
        
        await usersRepository.AddAsync(user, cancellationToken);

        return Result<UserDto>.Success(new UserDto(user.Id, user.Name));
    }
}