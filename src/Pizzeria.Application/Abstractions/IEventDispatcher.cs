namespace Pizzeria.Application.Abstractions;

public interface IEventDispatcher
{
    Task PublishAsync<TEvent>(
        string schemaId, 
        string messageKey, 
        TEvent @event, CancellationToken ct = default
    );
}