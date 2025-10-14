namespace Pizzeria.Application.Events;

public sealed record PizzaAddedEvent(int Id, string Name, string Size, decimal Price);

public sealed record PizzaRemovedEvent(int Id);