namespace LearningCSharp.Domain.Common;

public class AggregateRoot
{
    private readonly List<object> _events = new();
    public IReadOnlyCollection<object> Events => _events.AsReadOnly();

    protected void AddDomainEvent(object @event) => _events.Add(@event);
    public void ClearDomainEvents() => _events.Clear();
}