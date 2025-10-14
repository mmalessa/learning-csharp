using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pizzeria.Application.Abstractions;

namespace Pizzeria.Infrastructure.Outbox;

public sealed class OutboxProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly string _topic;
    private readonly TimeSpan _interval;
    private readonly int _batchSize;

    public OutboxProcessor(
        IServiceProvider serviceProvider,
        string bootstrapServers,
        string topic,
        ILogger<OutboxProcessor> logger,
        TimeSpan? interval = null,
        int batchSize = 100)
    {
        _serviceProvider = serviceProvider;
        _topic = topic;
        _logger = logger;
        _interval = interval ?? TimeSpan.FromSeconds(5);
        _batchSize = batchSize;

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            Acks = Acks.All
        };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Processor started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox messages");
            }

            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("Outbox Processor stopped");
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var outboxRepo = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var messages = await outboxRepo.GetUnprocessedMessagesAsync(_batchSize, ct);

        foreach (var message in messages)
        {
            try
            {
                var kafkaMessage = new Message<string, string>
                {
                    Key = message.MessageKey,
                    Value = message.Payload,
                    Headers = new Headers
                    {
                        { "schemaId", Encoding.UTF8.GetBytes(message.SchemaId) }
                    }
                };

                await _producer.ProduceAsync(_topic, kafkaMessage, ct);

                message.MarkAsProcessed();
                await outboxRepo.UpdateAsync(message, ct);

                _logger.LogInformation(
                    "Processed outbox message {MessageId} with schema {SchemaId}",
                    message.Id,
                    message.SchemaId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
                message.MarkAsFailed(ex.Message);
                await outboxRepo.UpdateAsync(message, ct);
            }
        }

        if (messages.Count > 0)
        {
            await uow.SaveChangesAsync(ct);
        }
    }

    public override void Dispose()
    {
        _producer.Dispose();
        base.Dispose();
    }
}
