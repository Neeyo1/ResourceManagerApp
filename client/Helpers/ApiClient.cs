using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;
using Blazored.Toast.Services;
using shared.DTOs.Account;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using static client.Helpers.JwtHelper;

namespace client.Helpers;

public class ApiClient(HttpClient httpClient, ILocalStorageService localStorage, IToastService toastService)
{
    public async Task<HttpResponseMessage> GetAsync(string url)
    {
        await AddTokenToRequest();
        return await httpClient.GetAsync(url);
    }

    public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string url, T model)
    {
        await AddTokenToRequest();
        return await httpClient.PostAsJsonAsync(url, model);
    }

    public async Task<HttpResponseMessage> PostAsJsonWithCredentialsAsync<T>(string url, T model)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
        };

        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        return await httpClient.SendAsync(request);
    }

    public async Task<HttpResponseMessage> PutAsJsonAsync<T>(string url, T model)
    {
        await AddTokenToRequest();
        return await httpClient.PutAsJsonAsync(url, model);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        await AddTokenToRequest();
        return await httpClient.DeleteAsync(url);
    }

    private async Task AddTokenToRequest()
    {
        var token = await localStorage.GetItemAsStringAsync("accessToken");
        if (token == null)
        {
            toastService.ShowError("Failed to get access token");
            return;
        }
        if (IsTokenExpired(token))
        {
            var newToken = await RefreshTokenAsync(token);
            if (newToken == null)
            {
                toastService.ShowError("Failed to refresh access token");
                return;
            }
            else
            {
                token = newToken;
                await localStorage.SetItemAsStringAsync("accessToken", token);
            }
        }

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<string?> RefreshTokenAsync(string token)
    {
        var tokenDto = new TokenDto
        {
            Token = token
        };

        var response = await PostAsJsonWithCredentialsAsync("account/refresh", tokenDto);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<UserDto>();
        if (result == null)
        {
            return null;
        }
        return result.Token;
    }
}
