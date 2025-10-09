namespace LearningCSharp.Api.Orders;

public record CreateOrderRequest(
    Guid OrderId,
    string CustomerId,
    decimal Total
);