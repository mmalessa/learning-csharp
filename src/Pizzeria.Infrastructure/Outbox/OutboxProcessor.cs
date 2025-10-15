using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pizzeria.Application.Abstractions;

namespace Pizzeria.Infrastructure.Outbox;

public sealed class OutboxProcessor : BackgroundService
{
    private readonly IOutboxBatchProcessor _batchProcessor;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly TimeSpan _interval;
    private readonly int _batchSize;

    public OutboxProcessor(
        IOutboxBatchProcessor batchProcessor,
        ILogger<OutboxProcessor> logger,
        TimeSpan? interval = null,
        int batchSize = 100)
    {
        _batchProcessor = batchProcessor ?? throw new ArgumentNullException(nameof(batchProcessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _interval = interval ?? TimeSpan.FromSeconds(5);
        _batchSize = batchSize;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Processor started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var processed = await _batchProcessor.ProcessAsync(_batchSize, stoppingToken);
                if (processed == 0)
                {
                    await Task.Delay(_interval, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox messages");
                await Task.Delay(_interval, stoppingToken);
            }
        }

        _logger.LogInformation("Outbox Processor stopped");
    }

    public override void Dispose()
    {
        if (_batchProcessor is IDisposable disposable)
        {
            disposable.Dispose();
        }

        base.Dispose();
    }
}
