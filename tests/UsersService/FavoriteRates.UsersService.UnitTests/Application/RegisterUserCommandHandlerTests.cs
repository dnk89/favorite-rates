using FavoriteRates.UsersService.Application.Abstractions;
using FavoriteRates.UsersService.Application.Users.Register;
using FavoriteRates.UsersService.Domain.Entities;
using FavoriteRates.UsersService.Domain.Repositories;
using Moq;

namespace FavoriteRates.UsersService.UnitTests.Application;

public class RegisterUserCommandHandlerTests
{
    private readonly RegisterUserCommandHandler _sut;
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IUsersRepository> _userRepository = new();
    
    public RegisterUserCommandHandlerTests()
    {
        _sut = new RegisterUserCommandHandler(_passwordHasher.Object, _userRepository.Object);
    }

    [Fact]
    public async Task Handle_HappyPath_AddsUserAndReturnsSuccess()
    {
        var ct = CancellationToken.None;
        var validCommand = new RegisterUserCommand("TESTUSER", "password", "password");
        var normalizedName = validCommand.Name.ToLower();
        _userRepository
            .Setup(x => x.ExistsWithNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        const string expectedHash = "hashedpassword";
        _passwordHasher
            .Setup(x => x.Hash(It.IsAny<string>()))
            .Returns(expectedHash);
        
        var result = await _sut.Handle(validCommand, ct);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(normalizedName, result.Value.Name);
        _userRepository.Verify(x => x.ExistsWithNameAsync(validCommand.Name, ct), Times.Once);
        _userRepository.Verify(x => x.AddAsync(
            It.Is<User>(u => u.Name == normalizedName && u.PasswordHash == expectedHash && u.Id == result.Value.Id), ct), Times.Once);
    }
    
    [Fact]
    public async Task Handle_UserAlreadyExists_ReturnsFailure()
    {
        var ct = CancellationToken.None;
        var validCommand = new RegisterUserCommand("testuser", "password", "password");
        _userRepository
            .Setup(x => x.ExistsWithNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var result = await _sut.Handle(validCommand, ct);
        
        Assert.False(result.IsSuccess);
        Assert.True(!string.IsNullOrEmpty(result.Error));
        _userRepository.Verify(x => x.ExistsWithNameAsync(validCommand.Name, ct), Times.Once);
    }
}