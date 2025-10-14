using System.Text.Json;
using Confluent.Kafka;
using Pizzeria.Application.Abstractions;

namespace Pizzeria.Infrastructure.Kafka;

public sealed class KafkaEventDispatcher : IEventDispatcher, IDisposable 
{
    private readonly IProducer<string, string> _producer;

    public KafkaEventDispatcher(string bootstrapServers)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            Acks = Acks.All
        };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync<TEvent>(string topic, TEvent @event, CancellationToken ct = default)
    {
        var message = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(@event)
        };

        await _producer.ProduceAsync(topic, message, ct);
    }

    public void Dispose() => _producer.Dispose();
}