using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PUSH_SERVER_WEB;
using PUSH_SERVER_WEB.Services;
using Blazored.Modal;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
     .AddScoped<IAuthenticationService, AuthService>()
     .AddScoped<IHttpService, HttpService>()
     .AddScoped<ILocalStorageService, LocalStorageService>()
     .AddScoped<IProjectService, ProjectService>();
builder.Services.AddBlazoredModal();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
