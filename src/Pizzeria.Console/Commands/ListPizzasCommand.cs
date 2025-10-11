using Mediator;
using Pizzeria.Application.Pizzas;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pizzeria.Console.Commands;

public sealed class ListPizzasCommand(IMediator mediator) : AsyncCommand<ListPizzasCommand.Settings>
{
    public sealed class Settings : CommandSettings { }

    public override async Task<int> ExecuteAsync(CommandContext _, Settings __)
    {
        var items = await mediator.Send(new ListPizzas());
        var table = new Table { Border = TableBorder.Rounded };
        table.AddColumn("Id"); table.AddColumn("Nazwa"); table.AddColumn("Rozmiar"); table.AddColumn("Cena");

        foreach (var p in items)
            table.AddRow(p.Id.ToString(), p.Name, p.Size.ToString(), p.Price.ToString("0.00"));

        AnsiConsole.Write(table);
        return 0;
    }
}
