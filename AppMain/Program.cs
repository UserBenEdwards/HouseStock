using AppMain.DIExtensions;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDb(context.Configuration);
    })
    .Build();

Console.WriteLine("Hello, World!");

await host.RunAsync();