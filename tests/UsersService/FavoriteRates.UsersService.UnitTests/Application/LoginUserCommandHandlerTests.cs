using FavoriteRates.UsersService.Application.Abstractions;
using FavoriteRates.UsersService.Application.Users.Login;
using FavoriteRates.UsersService.Domain.Entities;
using FavoriteRates.UsersService.Domain.Services;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FavoriteRates.UsersService.UnitTests.Application;

public class LoginUserCommandHandlerTests
{
    private readonly LoginUserCommandHandler _sut;

    private readonly Mock<IValidator<LoginUserCommand>> _validator = new();
    private readonly Mock<IUsersRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IUserTokenProvider> _tokenProvider = new();
    
    public LoginUserCommandHandlerTests()
    {
        _sut = new LoginUserCommandHandler(
            _validator.Object, 
            _userRepository.Object, 
            _passwordHasher.Object, 
            _tokenProvider.Object);
    }
    
    [Fact]
    public async Task Handle_InvalidCommand_ReturnsFailureWithFirstError()
    {
        var command = new LoginUserCommand("", "");
        const string expectedError = "Name is required.";
        const string secondError = "Password is required.";
        _validator
            .Setup(x => x.ValidateAsync(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([
                new ValidationFailure("Name", expectedError),
                new ValidationFailure("Password", secondError)
            ]));
        var ct = CancellationToken.None;
        
        var result = await _sut.Handle(command, ct);
        
        Assert.False(result.IsSuccess);
        Assert.Equal(expectedError, result.Error);
        _validator.Verify(x => x.ValidateAsync(command, ct), Times.Once);
        _userRepository.Verify(x => x.FindByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _passwordHasher.Verify(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _tokenProvider.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UserNotExists_ReturnsFailure()
    {
        var command = new LoginUserCommand("John", "password");
        _validator
            .Setup(x => x.ValidateAsync(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _userRepository
            .Setup(x => x.FindByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        var ct = CancellationToken.None;
        
        var result = await _sut.Handle(command, ct);
        
        Assert.False(result.IsSuccess);
        Assert.True(!string.IsNullOrEmpty(result.Error));
        _validator.Verify(x => x.ValidateAsync(command, ct), Times.Once);
        _userRepository.Verify(x => x.FindByNameAsync(command.Name, ct), Times.Once);
        _passwordHasher.Verify(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _tokenProvider.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_PasswordNotMatch_ReturnsFailure()
    {
        var command = new LoginUserCommand("John", "password");
        _validator
            .Setup(x => x.ValidateAsync(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            PasswordHash = "invalidhash"
        };
        _userRepository
            .Setup(x => x.FindByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher
            .Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);
        var ct = CancellationToken.None;
        
        var result = await _sut.Handle(command, ct);
        
        Assert.False(result.IsSuccess);
        Assert.True(!string.IsNullOrEmpty(result.Error));
        _validator.Verify(x => x.ValidateAsync(command, ct), Times.Once);
        _userRepository.Verify(x => x.FindByNameAsync(command.Name, ct), Times.Once);
        _passwordHasher.Verify(x => x.Verify(command.Password, user.PasswordHash), Times.Once);
        _tokenProvider.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
    }
    
    [Fact]
    public async Task Handle_HappyPath_ReturnsSuccessWithToken()
    {
        var command = new LoginUserCommand("John", "password");
        _validator
            .Setup(x => x.ValidateAsync(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            PasswordHash = "hashedpassword"
        };
        _userRepository
            .Setup(x => x.FindByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher
            .Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);
        const string expectedToken = "token";
        _tokenProvider
            .Setup(x => x.GenerateToken(user))
            .Returns(expectedToken);
        var ct = CancellationToken.None;
        
        var result = await _sut.Handle(command, ct);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(expectedToken, result.Value.Token);
        _validator.Verify(x => x.ValidateAsync(command, ct), Times.Once);
        _userRepository.Verify(x => x.FindByNameAsync(command.Name, ct), Times.Once);
        _passwordHasher.Verify(x => x.Verify(command.Password, user.PasswordHash), Times.Once);
        _tokenProvider.Verify(x => x.GenerateToken(user), Times.Once);
    }
}