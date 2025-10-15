using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pizzeria.Application.Abstractions;

namespace Pizzeria.Infrastructure.Outbox;

public sealed class OutboxBatchProcessor : IOutboxBatchProcessor, IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;
    private readonly ILogger<OutboxBatchProcessor> _logger;

    public OutboxBatchProcessor(
        IServiceScopeFactory scopeFactory,
        ProducerConfig config,
        string topic,
        ILogger<OutboxBatchProcessor> logger)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _topic = string.IsNullOrWhiteSpace(topic) ? throw new ArgumentException("Topic must be provided.", nameof(topic)) : topic;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _producer = new ProducerBuilder<string, string>(config ?? throw new ArgumentNullException(nameof(config))).Build();
    }

    public async Task<int> ProcessAsync(int batchSize, CancellationToken ct)
    {
        if (batchSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be greater than zero.");
        }

        using var scope = _scopeFactory.CreateScope();
        var outboxRepo = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var messages = await outboxRepo.GetUnprocessedMessagesAsync(batchSize, ct);

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
                    "Published outbox message {MessageId} with schema {SchemaId}",
                    message.Id,
                    message.SchemaId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish outbox message {MessageId}", message.Id);
                message.MarkAsFailed(ex.Message);
                await outboxRepo.UpdateAsync(message, ct);
            }
        }

        if (messages.Count > 0)
        {
            await uow.SaveChangesAsync(ct);
        }

        return messages.Count;
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}
