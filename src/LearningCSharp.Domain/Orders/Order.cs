namespace LearningCSharp.Domain.Orders;

public class Order
{
    public Guid Id { get; private set; }
    public string CustomerId { get; private set; }
    public decimal Total { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Order() { } // EF

    private Order(Guid id, string customerId, decimal total, DateTime createdAt)
    {
        Id = id;
        CustomerId = customerId;
        Total = total;
        CreatedAt = createdAt;

        // AddDomainEvent(new OrderCreated(Id, CustomerId, Total));
    }

    public static Order Create(Guid id, string customerId, decimal total, DateTime createdAt) 
        => new Order(id, customerId, total, createdAt);
}