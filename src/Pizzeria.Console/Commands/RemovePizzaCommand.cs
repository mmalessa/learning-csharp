using Mediator;
using Pizzeria.Application.Pizzas;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pizzeria.Console.Commands;

public sealed class RemovePizzaCommand(IMediator mediator) : AsyncCommand<RemovePizzaCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<ID>")] public int Id { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext _, Settings s)
    {
        var ok = await mediator.Send(new RemovePizza(s.Id));
        AnsiConsole.MarkupLine(ok ? "[yellow]Usunięto[/]." : "[red]Nie znaleziono[/].");
        return ok ? 0 : 2;
    }
}
