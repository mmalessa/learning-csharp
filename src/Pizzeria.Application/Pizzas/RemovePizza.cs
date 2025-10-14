using Mediator;
using Pizzeria.Application.Abstractions;
using Pizzeria.Application.Events;

namespace Pizzeria.Application.Pizzas;

public sealed record RemovePizza(int Id) : IRequest<bool>; // true gdy usunięto

public sealed class RemovePizzaHandler(
    IPizzaRepository repo, 
    IUnitOfWork uow,
    IEventDispatcher events
) : IRequestHandler<RemovePizza, bool>
{
    public async ValueTask<bool> Handle(RemovePizza request, CancellationToken ct)
    {
        var entity = await repo.GetAsync(request.Id, ct);
        if (entity is null) return false;
        await repo.RemoveAsync(entity, ct);
        await uow.SaveChangesAsync(ct);

        var evt = new PizzaRemovedEvent(entity.Id);
        var messageKey = entity.Id.ToString();
        const string schemaId = "mm.pizzeria.pizza-removed";
        await events.PublishAsync(schemaId, messageKey, evt, ct);
        
        return true;
    }
}