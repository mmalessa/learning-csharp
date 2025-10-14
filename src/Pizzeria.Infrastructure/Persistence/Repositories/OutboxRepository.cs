using Microsoft.EntityFrameworkCore;
using Pizzeria.Application.Abstractions;
using Pizzeria.Domain;

namespace Pizzeria.Infrastructure.Persistence.Repositories;

public sealed class OutboxRepository(PizzeriaDbContext db) : IOutboxRepository
{
    public Task AddAsync(OutboxMessage message, CancellationToken ct)
        => db.OutboxMessages.AddAsync(message, ct).AsTask();

    public async Task<IReadOnlyList<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize, CancellationToken ct)
    {
        return await db.OutboxMessages
            .Where(m => m.ProcessedAt == null)
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync(ct);
    }

    public Task UpdateAsync(OutboxMessage message, CancellationToken ct)
    {
        db.OutboxMessages.Update(message);
        return Task.CompletedTask;
    }
}
