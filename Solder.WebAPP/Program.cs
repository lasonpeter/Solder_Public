using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Solder.Core.Interfaces.Api.Solder;
using Solder.Infrastructure.Persistence.SolderAPI;
using Solder.Shared.Statics;
using Solder.WebAPP;
using SolderAuthSessionStore = Solder.WebAPP.Services.SolderAuthSessionStore;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["Api:BaseUrl"];
if (string.IsNullOrWhiteSpace(apiBaseUrl)) apiBaseUrl = SolderUris.FallbackBaseUrl;

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
builder.Services.AddScoped<ISolderAuthSessionStore, SolderAuthSessionStore>();
builder.Services.AddScoped<ISolderAuthService, SolderAuthService>();
builder.Services.AddScoped<ISolderServerApiService, SolderServerApiService>();

await builder.Build().RunAsync();