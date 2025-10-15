using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pizzeria.Application.Abstractions;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pizzeria.Console.Commands;

public sealed class OutboxConsumeCommand(
    IOutboxBatchProcessor batchProcessor,
    ILogger<OutboxConsumeCommand> logger) : AsyncCommand<OutboxConsumeCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("-b|--batch-size <SIZE>")]
        [Description("Number of messages to process per batch (default: 100).")]
        [DefaultValue(100)]
        public int BatchSize { get; init; } = 100;

        public override ValidationResult Validate()
            => BatchSize <= 0
                ? ValidationResult.Error("Batch size must be greater than zero.")
                : ValidationResult.Success();
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var total = 0;
        var iteration = 0;

        while (true)
        {
            var processed = await batchProcessor.ProcessAsync(settings.BatchSize, CancellationToken.None);
            if (processed == 0)
            {
                logger.LogInformation("Outbox drain complete after {Iterations} iteration(s).", iteration);
                break;
            }

            iteration++;
            total += processed;
            logger.LogInformation("Processed {Count} outbox message(s) in iteration {Iteration}.", processed, iteration);
        }

        if (total > 0)
        {
            AnsiConsole.MarkupLine($"[green]OK[/] Published {total} message(s) to Kafka.");
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]No pending outbox messages found.[/]");
        }

        return 0;
    }
}
