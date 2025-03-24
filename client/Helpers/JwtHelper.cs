using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace client.Helpers;

public static class JwtHelper
{
    public static IEnumerable<Claim> ParseClaimsFromJwt(string token)
    {
        var jwt = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
        if (jwt == null)
        {
            return [];
        }
        return jwt.Claims;
    }

    public static bool IsTokenExpired(string token)
    {
        var jwt = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
        if (jwt == null)
        {
            return false;
        }
        return jwt.ValidTo < DateTime.UtcNow;
    }
}
