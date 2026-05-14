using Microsoft.Extensions.DependencyInjection;
using View;

namespace AppMain.DIExtensions;

public static class PresentationExtension
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddTransient<ConsolePresentation>();
        return services;
    }
}
