using AppMain.DIExtensions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Service.Options;
using View;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(
        new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build())
    .CreateLogger();

try
{
    Log.Information("Starting Housestock application");

    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureServices((context, services) =>
        {
            services.AddDb(context.Configuration);
            services.AddRepositories();
            services.AddAppServices();
            services.AddControllers();
            services.AddPresentation();
            services.Configure<AuthOptions>(context.Configuration.GetSection("Auth"));
        })
        .Build();

    // Применяем миграции и заполняем БД при первом запуске
    var db = host.Services.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);

    // Запускаем консольный цикл
    var presentation = host.Services.GetRequiredService<ConsolePresentation>();
    await presentation.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
