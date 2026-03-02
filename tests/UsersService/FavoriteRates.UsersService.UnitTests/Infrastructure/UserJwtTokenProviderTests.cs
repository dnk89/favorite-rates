using System.Security.Claims;
using FavoriteRates.UsersService.Domain.Entities;
using FavoriteRates.UsersService.Infrastructure.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;

namespace FavoriteRates.UsersService.UnitTests.Infrastructure;

public class UserJwtTokenProviderTests
{
    private readonly UserJwtTokenProvider _sut;
    private readonly Mock<IOptions<JwtOptions>> _jwtOptions = new();
    private readonly JwtOptions _options;

    public UserJwtTokenProviderTests()
    {
        _options = new JwtOptions
        {
            Secret = "this-is-a-very-secret-key-that-is-at-least-32-characters-long",
            ExpirationInMinutes = 30,
            Issuer = "issuer",
            Audience = "audience"
        };
        _jwtOptions.Setup(x => x.Value).Returns(_options);
        _sut = new UserJwtTokenProvider(_jwtOptions.Object);
    }

    [Fact]
    public void GenerateToken_ValidUser_ReturnsValidJwt()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "TestUser"
        };

        var token = _sut.GenerateToken(user);

        Assert.NotNull(token);
        Assert.NotEmpty(token);

        var handler = new JsonWebTokenHandler();
        var jwtToken = handler.ReadJsonWebToken(token);

        Assert.Equal(user.Id.ToString(), jwtToken.Subject);
        Assert.Equal(user.Name, jwtToken.GetClaim(JwtRegisteredClaimNames.Name).Value);
        Assert.Equal(_options.Issuer, jwtToken.Issuer);
        Assert.Contains(_options.Audience, jwtToken.Audiences);
    }
}
