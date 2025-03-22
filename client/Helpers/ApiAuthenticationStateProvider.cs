using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace client.Helpers;

public class ApiAuthenticationStateProvider(IJSRuntime jsRuntime) : AuthenticationStateProvider
{
    public async Task MarkUserAsAuthenticated(string token)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", "accessToken", token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task MarkUserAsLoggedOut()
    {
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "accessToken");
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await jsRuntime.InvokeAsync<string>("localStorage.getItem", "accessToken");
        if (string.IsNullOrEmpty(token))
        {
            var currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            return new AuthenticationState(currentUser);
        }

        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = Convert.FromBase64String(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        if (keyValuePairs == null)
        {
            return [];
        }

        return keyValuePairs
            .Where(kvp => kvp.Value != null)
            .Select(kvp => new Claim(kvp.Key, kvp.Value?.ToString() ?? ""));
    }
}
