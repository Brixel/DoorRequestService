using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

using Web.Services;

namespace Web.Extensions;

public static class DoorServiceExtensions
{
    public static void AddDoorService(this IServiceCollection services)
    {
        services.AddScoped<IDoorService, DoorService>();

        services.AddTransient<DoorRequestApiAuthorizationMessageHandler>();

        services.AddHttpClient<IDoorService, DoorService>(options =>
        {
            options.BaseAddress = new Uri("https://localhost:5001/api/DoorRequest/");
        }).AddHttpMessageHandler<DoorRequestApiAuthorizationMessageHandler>();
    }
}

public class DoorRequestApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public DoorRequestApiAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigation) : base(provider, navigation)
    {
        ConfigureHandler(
            authorizedUrls: new[] { "https://localhost:5001/api/DoorRequest/" });
    }
}