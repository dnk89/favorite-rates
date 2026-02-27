using FavoriteRates.UsersService.Domain.Entities;
using FluentValidation;

namespace FavoriteRates.UsersService.Application.Users.Register;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(User.MinNameLength).WithMessage($"Name must be at least {User.MinNameLength} characters long.")
            .MaximumLength(User.MaxNameLength).WithMessage($"Name must be at most {User.MaxNameLength} characters long.");
        
        RuleFor(x => x.Password1)
            .NotEmpty().WithMessage("Password1 is required.")
            .MinimumLength(User.MinPasswordLength).WithMessage($"Password1 must be at least {User.MinPasswordLength} characters long.");

        RuleFor(x => x.Password2)
            .NotEmpty().WithMessage("Password2 is required.")
            .MinimumLength(User.MinPasswordLength).WithMessage($"Password2 must be at least {User.MinPasswordLength} characters long.")
            .Equal(x => x.Password1).WithMessage("Passwords must match.");
    }
}