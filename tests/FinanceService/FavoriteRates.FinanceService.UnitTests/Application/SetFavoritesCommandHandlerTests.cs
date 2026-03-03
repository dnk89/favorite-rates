using FavoriteRates.FinanceService.Application.Abstractions;
using FavoriteRates.FinanceService.Application.Currencies.SetFavorites;
using FavoriteRates.FinanceService.Domain.Entities;
using FavoriteRates.FinanceService.Domain.Repositories;

namespace FavoriteRates.FinanceService.UnitTests.Application;

public class SetFavoritesCommandHandlerTests
{
    private readonly SetFavoritesCommandHandler _sut;
    private readonly Mock<IUserContext> _userContext = new();
    private readonly Mock<IUserFavoritesRepository> _userFavoritesRepository = new();

    public SetFavoritesCommandHandlerTests()
    {
        _sut = new SetFavoritesCommandHandler(
            _userContext.Object,
            _userFavoritesRepository.Object);
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ReturnsFailure()
    {
        var command = new SetFavoritesCommand(["USD", "EUR"]);
        _userContext.Setup(x => x.GetCurrentUserId()).Returns((Guid?)null);
        var ct = CancellationToken.None;

        var result = await _sut.Handle(command, ct);

        Assert.False(result.IsSuccess);
        Assert.Equal("User not authenticated.", result.Error);
        _userContext.Verify(x => x.GetCurrentUserId(), Times.Once);
        _userFavoritesRepository.Verify(x => x.UpdateAllAsync(It.IsAny<UserFavorite[]>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_HappyPath_UpdatesFavoritesAndReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        var currencies = new[] { "usd", "EUR" };
        var command = new SetFavoritesCommand(currencies);
        _userContext.Setup(x => x.GetCurrentUserId()).Returns(userId);
        var ct = CancellationToken.None;

        var result = await _sut.Handle(command, ct);

        Assert.True(result.IsSuccess);
        _userContext.Verify(x => x.GetCurrentUserId(), Times.Once);
        _userFavoritesRepository.Verify(x => x.UpdateAllAsync(
            It.Is<UserFavorite[]>(f => 
                f.Length == 2 && 
                f.All(c => c.UserId == userId) &&
                f.Any(c => c.CurrencyId == "USD") &&
                f.Any(c => c.CurrencyId == "EUR")),
            ct), Times.Once);
    }
}