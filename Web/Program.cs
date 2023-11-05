using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Shared.Authorization;

using Web;
using Web.Authorization;
using Web.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddOidcAuthentication(options =>
{
    // Configure your authentication provider options here.
    // For more information, see https://aka.ms/blazor-standalone-auth
    builder.Configuration.Bind("Local", options.ProviderOptions);
    options.UserOptions.RoleClaim = CustomClaims.Roles;
}).AddAccountClaimsPrincipalFactory<CustomUserFactory>();

builder.Services.AddMudBlazor();

builder.Services.AddDoorService();

await builder.Build().RunAsync();
