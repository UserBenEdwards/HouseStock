using AppMain.DIExtensions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Options;
using View;

var host = Host.CreateDefaultBuilder(args)
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

// Применяем миграции при старте
var db = host.Services.GetRequiredService<AppDbContext>();
await db.Database.MigrateAsync();

// Запускаем консольный цикл
var presentation = host.Services.GetRequiredService<ConsolePresentation>();
await presentation.RunAsync();
