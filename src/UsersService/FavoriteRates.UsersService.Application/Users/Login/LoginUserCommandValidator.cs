using FluentValidation;

namespace FavoriteRates.UsersService.Application.Users.Login;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}