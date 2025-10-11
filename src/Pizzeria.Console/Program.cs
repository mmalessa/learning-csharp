using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;
using Pizzeria.Console.Commands;
using Pizzeria.Application;
using Pizzeria.Infrastructure;
using Pizzeria.Console; // <- dla ServiceProviderTypeRegistrar

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        var conn = Environment.GetEnvironmentVariable("PIZZERIA_DB");
        if (string.IsNullOrWhiteSpace(conn))
        {
            Console.WriteLine("ERROR: PIZZERIA_DB not set in environment variables.");
            Environment.Exit(1);
        }

        services.AddMediator();
        services.AddApplication();
        services.AddInfrastructure(conn);

        // Komendy – rejestrujemy w hostowym DI
        services.AddTransient<AddPizzaCommand>();
        services.AddTransient<RemovePizzaCommand>();
        services.AddTransient<ListPizzasCommand>();
    });

using var host = builder.Build();

// Tworzymy CommandApp dopiero TERAZ i wpinamy resolver z host.Services
var registrar = new ServiceProviderTypeRegistrar(host.Services);
var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.SetApplicationName("pizzeria");
    config.AddBranch("pizza", pizza =>
    {
        pizza.SetDescription("Operacje na pizzach");
        pizza.AddCommand<AddPizzaCommand>("add").WithDescription("Dodaj pizzę");
        pizza.AddCommand<RemovePizzaCommand>("remove").WithDescription("Usuń pizzę");
        pizza.AddCommand<ListPizzasCommand>("list").WithDescription("Lista pizz");
    });
});

return app.Run(args);