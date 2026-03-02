using FavoriteRates.SharedLibrary.ResultPattern;
using FavoriteRates.UsersService.Application.Abstractions;
using FavoriteRates.UsersService.Domain.Entities;
using FavoriteRates.UsersService.Domain.Repositories;
using FluentValidation;

namespace FavoriteRates.UsersService.Application.Users.Register;

public sealed class RegisterUserCommandHandler(
    IValidator<RegisterUserCommand> validator,
    IPasswordHasher passwordHasher,
    IUsersRepository usersRepository)
{
    public async Task<Result<RegisteredUserDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            var error = validation.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed.";
            return Result<RegisteredUserDto>.Failure(error);
        }
        
        if (await usersRepository.ExistsWithNameAsync(request.Name, cancellationToken))
        {
            return Result<RegisteredUserDto>.Failure("User with this name already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name.ToLower(),
            PasswordHash = passwordHasher.Hash(request.Password1)
        };
        
        await usersRepository.AddAsync(user, cancellationToken);

        return Result<RegisteredUserDto>.Success(new RegisteredUserDto(user.Id, user.Name));
    }
}