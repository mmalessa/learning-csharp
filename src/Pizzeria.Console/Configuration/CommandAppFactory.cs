using System;
using Pizzeria.Console.Commands;
using Spectre.Console.Cli;

namespace Pizzeria.Console.Configuration;

internal static class CommandAppFactory
{
    public static CommandApp Create(IServiceProvider services)
    {
        var registrar = new ServiceProviderTypeRegistrar(services);
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

            config.AddBranch("outbox", outbox =>
            {
                outbox.SetDescription("Operacje na kolejce outbox");
                outbox.AddCommand<OutboxConsumeCommand>("consume")
                    .WithDescription("Publikuje oczekujące wiadomości outbox do Kafka.");
            });
        });

        return app;
    }
}
