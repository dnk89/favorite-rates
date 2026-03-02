using System.Security.Claims;
using FavoriteRates.FinanceService.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;

namespace FavoriteRates.FinanceService.Infrastructure.Authentication;

public class UserContext(IHttpContextAccessor httpContextAccessor, ILogger<UserContext> logger) : IUserContext
{
    public Guid? GetCurrentUserId()
    {
        var claimsPrincipal = httpContextAccessor.HttpContext?.User;
        if (claimsPrincipal is null)
        {
            logger.LogDebug("ClaimsPrincipal is null.");
            return null;
        }

        var userId = claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            logger.LogDebug("User id is null.");
            var claims = claimsPrincipal?.Claims.Select(c => $"{c.Type}: {c.Value}").ToArray();
            logger.LogDebug("Claims: {claims}.", string.Join(", ", claims ?? []));
            return null;
        }

        if (!Guid.TryParse(userId, out var id) || id == Guid.Empty)
        {
            logger.LogDebug("User id is not valid. Value: {userId}.", id);
            return null;
        }

        return id;
    }
}