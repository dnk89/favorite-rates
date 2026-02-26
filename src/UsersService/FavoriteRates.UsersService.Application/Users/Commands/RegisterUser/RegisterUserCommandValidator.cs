using FluentValidation;

namespace FavoriteRates.UsersService.Application.Users.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long.");
        
        RuleFor(x => x.Password1)
            .NotEmpty().WithMessage("Password1 is required.")
            .MinimumLength(8).WithMessage("Password1 must be at least 8 characters long.");

        RuleFor(x => x.Password2)
            .NotEmpty().WithMessage("Password2 is required.")
            .MinimumLength(8).WithMessage("Password2 must be at least 8 characters long.")
            .Equal(x => x.Password1).WithMessage(
                "Passwords must match."
            );
    }
}