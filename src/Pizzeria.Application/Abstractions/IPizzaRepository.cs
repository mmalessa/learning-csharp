using Pizzeria.Domain;

namespace Pizzeria.Application.Abstractions;

public interface IPizzaRepository
{
    Task AddAsync(Pizza pizza, CancellationToken ct);
    Task<Pizza?> GetAsync(int id, CancellationToken ct);
    Task RemoveAsync(Pizza pizza, CancellationToken ct);
    IAsyncEnumerable<Pizza> ListAsync(CancellationToken ct);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct);
}