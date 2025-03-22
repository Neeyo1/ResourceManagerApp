using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace client.Helpers;

public class ApiClient(HttpClient httpClient, IJSRuntime jsRuntime)
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
        var token = await jsRuntime.InvokeAsync<string>("localStorage.getItem", "accessToken");
        if (token != null)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
