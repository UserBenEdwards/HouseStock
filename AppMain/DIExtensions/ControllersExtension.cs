using AppController;
using AppController.Commands.Admin;
using AppController.Commands.Query;
using AppController.Commands.System;
using AppController.Controllers;
using AppController.Providers;
using AppController.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace AppMain.DIExtensions;

public static class ControllersExtension
{
    public static IServiceCollection AddControllers(this IServiceCollection services)
    {
        // Session
        services.AddSingleton<AppSession>();

        // Utils
        services.AddTransient<RequestParser>();

        // Query commands
        services.AddTransient<FindCommand>();
        services.AddTransient<ShowCommand>();
        services.AddTransient<CostCommand>();
        services.AddTransient<StatsCommand>();
        services.AddTransient<HelpCommand>();
        services.AddTransient<ListCategoriesCommand>();
        services.AddTransient<SwitchToAdminCommand>();
        services.AddTransient<WrongCommand>();

        // Admin commands
        services.AddTransient<AddApplianceCommand>();
        services.AddTransient<UpdateApplianceCommand>();
        services.AddTransient<DeleteApplianceCommand>();
        services.AddTransient<AdminHelpCommand>();
        services.AddTransient<SwitchToUserCommand>();
        services.AddTransient<AddCategoryCommand>();
        services.AddTransient<UpdateCategoryCommand>();
        services.AddTransient<DeleteCategoryCommand>();

        // System commands
        services.AddTransient<ExitCommand>();

        // Providers
        services.AddTransient<QueryCommandProvider>();
        services.AddTransient<AdminCommandProvider>();
        services.AddTransient<CommandRegistry>();

        // Controllers
        services.AddTransient<QueryController>();
        services.AddTransient<AdminController>();

        return services;
    }
}
