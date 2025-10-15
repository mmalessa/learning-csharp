namespace Pizzeria.Application.Abstractions;

public interface IOutboxBatchProcessor
{
    Task<int> ProcessAsync(int batchSize, CancellationToken ct);
}
