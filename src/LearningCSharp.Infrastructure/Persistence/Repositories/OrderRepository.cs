using LearningCSharp.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace LearningCSharp.Infrastructure.Persistence.Repositories;

public class OrderRepository(AppDbContext context) : IOrderRepository
{
    public Task<Order?> GetAsync(Guid id, CancellationToken ct = default)
        => context.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);
    
    public Task AddAsync(Order order, CancellationToken ct = default)
        => context.Orders.AddAsync(order, ct).AsTask();
    
    public Task SaveChangesAsync(CancellationToken ct = default)
        => context.SaveChangesAsync(ct);
}
