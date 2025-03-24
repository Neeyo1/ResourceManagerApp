using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using client;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using client.Helpers;
using Blazored.LocalStorage;

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
        BaseAddress = new Uri("https://localhost:5001/api/")
    }
);
builder.Services.AddBlazoredToast();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireManagerRole", policy => policy.RequireRole("Manager", "Admin"));
});

await builder.Build().RunAsync();
