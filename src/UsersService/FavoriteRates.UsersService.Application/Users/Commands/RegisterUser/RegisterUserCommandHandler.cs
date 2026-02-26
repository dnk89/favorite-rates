using FavoriteRates.UsersService.Application.Common;
using FavoriteRates.UsersService.Application.Dtos;
using FluentValidation;

namespace FavoriteRates.UsersService.Application.Users.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler(IValidator<RegisterUserCommand> validator)
{
    public async Task<Result<UserDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            var error = validation.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed.";
            return Result<UserDto>.Failure(error);
        }
        
        // todo register user

        return Result<UserDto>.Success(new UserDto(Guid.NewGuid(), request.Name));
    }
}