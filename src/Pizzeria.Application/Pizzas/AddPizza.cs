using Mediator;
using Pizzeria.Application.Abstractions;
using Pizzeria.Application.Events;
using Pizzeria.Domain;

namespace Pizzeria.Application.Pizzas;

public sealed record AddPizza(int Id, string Name, PizzaSize Size, decimal Price) : IRequest<Unit>;

public sealed class AddPizzaHandler(
    IPizzaRepository repo, 
    IUnitOfWork uow,
    IEventDispatcher events
) : IRequestHandler<AddPizza, Unit>
{
    public async ValueTask<Unit> Handle(AddPizza request, CancellationToken ct)
    {
        var entity = new Pizza(request.Id, request.Name, request.Size, request.Price);
        await repo.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);
        
        var evt = new PizzaAddedEvent(entity.Id, entity.Name, entity.Size.ToString(), entity.Price);
        await events.PublishAsync("pizza-added", evt, ct);
        
        return Unit.Value;
    }
}