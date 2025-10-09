using LearningCSharp.Domain.Orders;

namespace LearningCSharp.Application.Orders;

public class CreateOrderHandler(IOrderRepository repository)
{
    public async Task HandleAsync(Guid orderId, string customerId, decimal total, DateTime createdAt, CancellationToken ct = default)
    {
        // używamy metody wytwórczej z domeny
        var order = Order.Create(orderId, customerId, total, createdAt);

        await repository.AddAsync(order, ct);
        await repository.SaveChangesAsync(ct);
        
    }
}