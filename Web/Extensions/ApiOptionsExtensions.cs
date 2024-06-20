using Web.Configuration;

namespace Web.Extensions;

public static class ApiOptionsExtensions
{
    public static IServiceCollection ConfigureApiOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ApiConfiguration>()
           .Bind(configuration.GetSection(ApiConfiguration.SectionName))
           .ValidateDataAnnotations();

        return services;
    }
}
