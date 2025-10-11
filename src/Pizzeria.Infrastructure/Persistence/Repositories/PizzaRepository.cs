using Microsoft.EntityFrameworkCore;
using Pizzeria.Application.Abstractions;
using Pizzeria.Domain;

namespace Pizzeria.Infrastructure.Persistence.Repositories;

public sealed class PizzaRepository(PizzeriaDbContext db) : IPizzaRepository
{
    public Task AddAsync(Pizza pizza, CancellationToken ct) => db.Pizzas.AddAsync(pizza, ct).AsTask();
    public Task<Pizza?> GetAsync(int id, CancellationToken ct) => db.Pizzas.FirstOrDefaultAsync(p => p.Id == id, ct);
    public Task RemoveAsync(Pizza pizza, CancellationToken ct) { db.Pizzas.Remove(pizza); return Task.CompletedTask; }
    
    public async IAsyncEnumerable<Pizza> ListAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        var pizzas = await db.Pizzas.AsNoTracking().ToListAsync(ct);
        foreach (var pizza in pizzas)
        {
            yield return pizza;
        }
    }
}

public sealed class UnitOfWork(PizzeriaDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}