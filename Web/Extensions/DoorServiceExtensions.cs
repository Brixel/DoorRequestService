using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Web.Configuration;
using Web.Services;

namespace Web.Extensions;

public static class DoorServiceExtensions
{
    public static void AddDoorService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ApiConfiguration>()
           .Bind(configuration.GetSection(ApiConfiguration.SectionName))
           .ValidateDataAnnotations();

        services.AddScoped<IDoorService, DoorService>();

        services.TryAddTransient<ApiAuthorizationMessageHandler>();

        services.AddHttpClient<IDoorService, DoorService>((provider, options) =>
        {
            var configuration = provider.GetRequiredService<IOptions<ApiConfiguration>>().Value;

            options.BaseAddress = new Uri($"{configuration.BaseUrl}/doorrequest/");
        }).AddHttpMessageHandler<ApiAuthorizationMessageHandler>();
    }
}

public class ApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public ApiAuthorizationMessageHandler(
        IAccessTokenProvider provider,
        NavigationManager navigation,
        IOptions<ApiConfiguration> configuration) : base(provider, navigation)
    {
        ConfigureHandler(
            authorizedUrls: new[] { configuration.Value.BaseUrl });
    }
}