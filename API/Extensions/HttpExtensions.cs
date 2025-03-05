namespace API.Extensions;

public static class HttpExtensions
{
    public static void SetRefreshToken(this HttpContext context, string refreshToken)
    {
        context.Response.Cookies.Append("refreshToken", refreshToken,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
    }

    public static string? GetRefreshToken(this HttpContext context)
    {
        context.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
        return refreshToken;
    }
}
