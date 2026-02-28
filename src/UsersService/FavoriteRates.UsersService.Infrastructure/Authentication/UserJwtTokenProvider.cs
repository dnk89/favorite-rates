using System.Security.Claims;
using System.Text;
using FavoriteRates.UsersService.Application.Abstractions;
using FavoriteRates.UsersService.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FavoriteRates.UsersService.Infrastructure.Authentication;

public class UserJwtTokenProvider(IOptions<JwtOptions> jwtOptions) : IUserTokenProvider
{
    public string GenerateToken(User user)
    {
        var jwt = jwtOptions.Value;
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Name)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(jwt.ExpirationInMinutes),
            SigningCredentials = credentials,
            Issuer = jwt.Issuer,
            Audience = jwt.Audience
        };

        var handler = new JsonWebTokenHandler();

        return handler.CreateToken(tokenDescriptor);
    }
}