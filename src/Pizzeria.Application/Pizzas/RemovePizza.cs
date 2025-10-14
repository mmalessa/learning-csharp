using System.Text.Json;
using Mediator;
using Pizzeria.Application.Abstractions;
using Pizzeria.Application.Events;
using Pizzeria.Domain;

namespace Pizzeria.Application.Pizzas;

public sealed record RemovePizza(int Id) : IRequest<bool>; // true gdy usunięto

public sealed class RemovePizzaHandler(
    IPizzaRepository repo,
    IOutboxRepository outboxRepo,
    IUnitOfWork uow
) : IRequestHandler<RemovePizza, bool>
{
    public async ValueTask<bool> Handle(RemovePizza request, CancellationToken ct)
    {
        var entity = await repo.GetAsync(request.Id, ct);
        if (entity is null) return false;
        await repo.RemoveAsync(entity, ct);

        var evt = new PizzaRemovedEvent(entity.Id);
        var messageKey = entity.Id.ToString();
        const string schemaId = "mm.pizzeria.pizza-removed";
        var payload = JsonSerializer.Serialize(evt);

        var outboxMessage = new OutboxMessage(schemaId, messageKey, payload);
        await outboxRepo.AddAsync(outboxMessage, ct);

        await uow.SaveChangesAsync(ct);

        return true;
    }
}