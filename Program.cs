using FrontendApp;
using FrontendApp.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7001") });

builder.Services.AddScoped<StorageService>();
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddScoped<ApiService>();

await builder.Build().RunAsync();
