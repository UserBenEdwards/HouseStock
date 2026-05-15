using AppController;
using AppController.Commands.Admin;
using AppController.Commands.Query;
using AppController.Commands.System;
using AppController.Controllers;
using AppController.Providers;
using AppController.Utils;
using Domain.Interfaces;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Service.Interfaces;
using Service.Options;
using Service.Services;

namespace Test.Integration;

public abstract class IntegrationTestBase
{
    protected static IServiceProvider CreateProvider(string? dbName = null)
    {
        var services = new ServiceCollection();

        services.AddLogging();

        // InMemory DB — каждый тест получает изолированную БД
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString()));

        // Repositories
        services.AddScoped<IApplianceRepository, ApplianceRepository>();
        services.AddScoped<IApplianceCategoryRepository, ApplianceCategoryRepository>();

        // Services
        services.AddScoped<IApplianceService, ApplianceService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IAuthService, AuthService>();
        services.Configure<AuthOptions>(opt => opt.AdminPassword = "admin123");

        // Session
        services.AddSingleton<AppSession>();

        // Utils
        services.AddTransient<RequestParser>();

        // Query commands
        services.AddTransient<FindCommand>();
        services.AddTransient<ShowCommand>();
        services.AddTransient<CostCommand>();
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

        // Providers & controllers
        services.AddTransient<QueryCommandProvider>();
        services.AddTransient<AdminCommandProvider>();
        services.AddTransient<CommandRegistry>();
        services.AddTransient<QueryController>();
        services.AddTransient<AdminController>();

        return services.BuildServiceProvider();
    }
}
