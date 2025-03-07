using System.Text.Json;
using API.Helpers;

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

    public static void AddPaginationHeader<T>(this HttpResponse response, PagedList<T> data)
    {
        var paginationHeader = new PaginationHeader(data.CurrentPage, data.PageSize, data.TotalCount,
            data.TotalPages);
        var jsonOptions = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
        response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader, jsonOptions));
        response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
    }
}
