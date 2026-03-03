using FavoriteRates.SharedLibrary.Application;
using FavoriteRates.SharedLibrary.ResultPattern;
using FavoriteRates.UsersService.Application.Abstractions;
using FavoriteRates.UsersService.Domain.Entities;
using FavoriteRates.UsersService.Domain.Repositories;

namespace FavoriteRates.UsersService.Application.Users.Register;

public sealed class RegisterUserCommandHandler(
    IPasswordHasher passwordHasher,
    IUsersRepository usersRepository) : IHandler<RegisterUserCommand, Result<RegisteredUserDto>>
{
    public async Task<Result<RegisteredUserDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
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