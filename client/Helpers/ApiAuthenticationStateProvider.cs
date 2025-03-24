using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace client.Helpers;

public class ApiAuthenticationStateProvider(ILocalStorageService localStorage) : AuthenticationStateProvider
{
    public async Task MarkUserAsAuthenticated(string token)
    {
        await localStorage.SetItemAsStringAsync("accessToken", token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task MarkUserAsLoggedOut()
    {
        await localStorage.RemoveItemAsync("accessToken");
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await localStorage.GetItemAsStringAsync("accessToken");
        if (string.IsNullOrEmpty(token) || IsTokenExpired(token))
        {
            var currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            return new AuthenticationState(currentUser);
        }

        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string token)
    {
        var jwt = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
        if (jwt == null)
        {
            return [];
        }
        return jwt.Claims;
    }

    private static bool IsTokenExpired(string token)
    {
        var jwt = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
        if (jwt == null)
        {
            return false;
        }
        return jwt.ValidTo < DateTime.UtcNow;
    }
}
