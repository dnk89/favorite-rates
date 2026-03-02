using System.Security.Claims;
using FavoriteRates.FinanceService.Infrastructure.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FavoriteRates.FinanceService.UnitTests.Infrastructure;

public class UserContextTests
{
    private readonly UserContext _sut;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
    private readonly Mock<ILogger<UserContext>> _logger = new();

    public UserContextTests()
    {
        _sut = new UserContext(_httpContextAccessor.Object, _logger.Object);
    }

    [Fact]
    public void GetCurrentUserId_HttpContextNull_ReturnsNull()
    {
        _httpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        var result = _sut.GetCurrentUserId();

        Assert.Null(result);
    }

    [Fact]
    public void GetCurrentUserId_UserNull_ReturnsNull()
    {
        var httpContext = new DefaultHttpContext { User = null! };
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var result = _sut.GetCurrentUserId();

        Assert.Null(result);
    }

    [Fact]
    public void GetCurrentUserId_UserIdClaimMissing_ReturnsNull()
    {
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var result = _sut.GetCurrentUserId();

        Assert.Null(result);
    }

    [Fact]
    public void GetCurrentUserId_UserIdInvalidGuid_ReturnsNull()
    {
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "not-a-guid")]);
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var result = _sut.GetCurrentUserId();

        Assert.Null(result);
    }

    [Fact]
    public void GetCurrentUserId_UserIdEmptyGuid_ReturnsNull()
    {
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, Guid.Empty.ToString())]);
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var result = _sut.GetCurrentUserId();

        Assert.Null(result);
    }

    [Fact]
    public void GetCurrentUserId_ValidUserId_ReturnsGuid()
    {
        var userId = Guid.NewGuid();
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId.ToString())]);
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var result = _sut.GetCurrentUserId();

        Assert.Equal(userId, result);
    }
}
