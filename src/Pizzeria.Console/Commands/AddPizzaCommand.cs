using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using Mediator;
using Pizzeria.Application.Pizzas;
using Pizzeria.Domain;

namespace Pizzeria.Console.Commands;

public sealed class AddPizzaCommand(IMediator mediator) : AsyncCommand<AddPizzaCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<ID>")]   [Description("Identyfikator pizzy (int).")] public int Id { get; init; }
        [CommandArgument(1, "<NAME>")] [Description("Nazwa pizzy.")] public string Name { get; init; } = default!;
        [CommandArgument(2, "<SIZE>")] [Description("Small|Medium|Large.")] public PizzaSize Size { get; init; }
        [CommandArgument(3, "<PRICE>")][Description("Cena.")] public decimal Price { get; init; }

        public override ValidationResult Validate()
            => Price <= 0 || string.IsNullOrWhiteSpace(Name)
                ? ValidationResult.Error("NAME i PRICE muszą być poprawne.")
                : ValidationResult.Success();
    }

    public override async Task<int> ExecuteAsync(CommandContext _, Settings s)
    {
        await mediator.Send(new AddPizza(s.Id, s.Name, s.Size, s.Price));
        AnsiConsole.MarkupLine("[green]OK[/] Dodano.");
        return 0;
    }
}