using Microsoft.Extensions.DependencyInjection;
using Service.Interfaces;
using Service.Services;

namespace AppMain.DIExtensions;

public static class ServicesExtension
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IApplianceService, ApplianceService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICategoryService, CategoryService>();
        return services;
    }
}
