using System.Security.Claims;

namespace CoreLayer.Utilities;

public static class UserUtil
{
    public static long GetUserId(this ClaimsPrincipal? claim)
    {
        var userId = claim?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return string.IsNullOrWhiteSpace(userId) ? 0 : Convert.ToInt64(userId);
    }
}