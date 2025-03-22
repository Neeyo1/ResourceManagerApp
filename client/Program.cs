using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using client;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using client.Helpers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped(sp => 
    new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5000/api/")
    }
);
builder.Services.AddBlazoredToast();

await builder.Build().RunAsync();
