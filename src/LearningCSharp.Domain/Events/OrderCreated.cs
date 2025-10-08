namespace LearningCSharp.Domain.Events;

public record OrderCreated(Guid OrderId, string CustomerId, decimal Total)
{
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}