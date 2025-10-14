using Pizzeria.Domain;

namespace Pizzeria.Application.Abstractions;

public interface IOutboxRepository
{
    Task AddAsync(OutboxMessage message, CancellationToken ct);
    Task<IReadOnlyList<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize, CancellationToken ct);
    Task UpdateAsync(OutboxMessage message, CancellationToken ct);
}
