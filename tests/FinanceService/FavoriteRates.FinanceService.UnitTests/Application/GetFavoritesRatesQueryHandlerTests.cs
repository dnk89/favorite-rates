using FavoriteRates.FinanceService.Application.Abstractions;
using FavoriteRates.FinanceService.Application.Currencies.GetFavoritesRates;
using FavoriteRates.FinanceService.Domain.Entities;
using FavoriteRates.FinanceService.Domain.Repositories;

namespace FavoriteRates.FinanceService.UnitTests.Application;

public class GetFavoritesRatesQueryHandlerTests
{
    private readonly GetFavoritesRatesQueryHandler _sut;
    private readonly Mock<ICurrenciesRepository> _currenciesRepository = new();
    private readonly Mock<IUserContext> _userContext = new();

    public GetFavoritesRatesQueryHandlerTests()
    {
        _sut = new GetFavoritesRatesQueryHandler(
            _currenciesRepository.Object,
            _userContext.Object);
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ReturnsFailure()
    {
        _userContext.Setup(x => x.GetCurrentUserId()).Returns((Guid?)null);
        var query = new GetFavoritesRatesQuery();
        var ct = CancellationToken.None;

        var result = await _sut.Handle(query, ct);

        Assert.False(result.IsSuccess);
        Assert.Equal("User not authenticated.", result.Error);
        _userContext.Verify(x => x.GetCurrentUserId(), Times.Once);
        _currenciesRepository.Verify(x => x.GetFavoritesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_HappyPath_ReturnsSuccessWithRates()
    {
        var userId = Guid.NewGuid();
        _userContext.Setup(x => x.GetCurrentUserId()).Returns(userId);

        var currencies = new List<Currency>
        {
            new() { Id = "USD", Name = "US Dollar", Rate = 1.0m },
            new() { Id = "EUR", Name = "Euro", Rate = 0.9m }
        };

        _currenciesRepository.Setup(x => x.GetFavoritesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currencies);

        var query = new GetFavoritesRatesQuery();
        var ct = CancellationToken.None;

        var result = await _sut.Handle(query, ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        var rates = result.Value.ToList();
        Assert.Equal(2, rates.Count);
        Assert.Contains(rates, x => x is { Code: "USD", Rate: 1.0m });
        Assert.Contains(rates, x => x is { Code: "EUR", Rate: 0.9m });
        
        _userContext.Verify(x => x.GetCurrentUserId(), Times.Once);
        _currenciesRepository.Verify(x => x.GetFavoritesAsync(userId, ct), Times.Once);
    }
}
