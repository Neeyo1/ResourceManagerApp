using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using static client.Helpers.JwtHelper;

namespace client.Helpers;

public class ApiAuthenticationStateProvider(ILocalStorageService localStorage, ApiClient apiClient)
    : AuthenticationStateProvider
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
        if (token == null)
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            return new AuthenticationState(anonymousUser);
        }
        if (IsTokenExpired(token))
        {
            var newToken = await apiClient.RefreshTokenAsync(token);
            if (newToken == null)
            {
                var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
                return new AuthenticationState(anonymousUser);
            }
            else
            {
                token = newToken;
                await localStorage.SetItemAsStringAsync("accessToken", token);
            }
        }

        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);
    }
}
