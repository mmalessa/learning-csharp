using System.Text.Json;
using Confluent.Kafka;
using Pizzeria.Application.Abstractions;

namespace Pizzeria.Infrastructure.Kafka;

public sealed class KafkaEventDispatcher : IEventDispatcher, IDisposable 
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;

    public KafkaEventDispatcher(string bootstrapServers, string topic)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            Acks = Acks.All
        };
        _producer = new ProducerBuilder<string, string>(config).Build();
        _topic = topic;
    }

    public async Task PublishAsync<TEvent>(
        string schemaId, 
        string messageKey, 
        TEvent @event, CancellationToken ct = default
    )
    {
        var message = new Message<string, string>
        {
            Key = messageKey,
            Value = JsonSerializer.Serialize(@event),
            Headers = new Headers
            {
                { "schemaId", System.Text.Encoding.UTF8.GetBytes(schemaId) }
            }
        };

        await _producer.ProduceAsync(_topic, message, ct);
    }

    public void Dispose() => _producer.Dispose();
}