namespace Pizzeria.Application.Abstractions;

public interface IEventDispatcher
{
    Task PublishAsync<TEvent>(string topic, TEvent @event, CancellationToken ct = default);
}