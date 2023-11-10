using Microsoft.Extensions.Options;

using Web.Configuration;
using Web.Services;

namespace Web.Extensions;

public static class ApiServiceExtensions
{
    public static void AddApiService(this IServiceCollection services)
    {
        services.AddScoped<IApiService, ApiService>();

        services.AddHttpClient<IApiService, ApiService>((provider, options) =>
        {
            var configuration = provider.GetRequiredService<IOptions<ApiConfiguration>>().Value;

            options.BaseAddress = new Uri(configuration.BaseUrl);
        });
    }
}