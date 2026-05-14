using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AppMain.DIExtensions;

public static class RepositoriesExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IApplianceRepository, ApplianceRepository>();
        services.AddScoped<IApplianceCategoryRepository, ApplianceCategoryRepository>();
        return services;
    }
}
