using FavoriteRates.UsersService.Application.Users.Commands.RegisterUser;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FavoriteRates.UsersService.UnitTests.Application;

public class RegisterUserCommandHandlerTests
{
    private readonly RegisterUserCommandHandler _sut;
    private readonly Mock<IValidator<RegisterUserCommand>> _validator;
    
    public RegisterUserCommandHandlerTests()
    {
        _validator = new Mock<IValidator<RegisterUserCommand>>();
        _sut = new RegisterUserCommandHandler(_validator.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        var ct = CancellationToken.None;
        var validCommand = new RegisterUserCommand("testuser", "password", "password");
        _validator
            .Setup(x => x.ValidateAsync(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        
        var result = await _sut.Handle(validCommand, ct);
        
        Assert.True(result.IsSuccess);
        _validator.Verify(x => x.ValidateAsync(validCommand, ct), Times.Once);
    }
    
    [Fact]
    public async Task Handle_InvalidCommand_ReturnsFailureWithFirstError()
    {
        var ct = CancellationToken.None;
        var invalidCommand = new RegisterUserCommand("", "", "");
        const string firstError = "Name is required.";
        const string secondError = "Password is required.";
        _validator
            .Setup(x => x.ValidateAsync(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([
                new ValidationFailure("Name", firstError),
                new ValidationFailure("Password", secondError)
            ]));
        
        var result = await _sut.Handle(invalidCommand, ct);
        
        Assert.False(result.IsSuccess);
        Assert.Equal(firstError, result.Error);
        _validator.Verify(x => x.ValidateAsync(invalidCommand, ct), Times.Once);   
    }
}