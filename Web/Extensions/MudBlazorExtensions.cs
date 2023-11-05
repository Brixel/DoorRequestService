using MudBlazor;
using MudBlazor.Services;

namespace Web.Extensions;

public static class MudBlazorExtensions
{
    public static IServiceCollection AddMudBlazor(this IServiceCollection services)
    {
        return services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
        });
    }
}
