using System.Text.Json;
using Mediator;
using Pizzeria.Application.Abstractions;
using Pizzeria.Application.Events;
using Pizzeria.Domain;

namespace Pizzeria.Application.Pizzas;

public sealed record AddPizza(int Id, string Name, PizzaSize Size, decimal Price) : IRequest<Unit>;

public sealed class AddPizzaHandler(
    IPizzaRepository repo,
    IOutboxRepository outboxRepo,
    IUnitOfWork uow
) : IRequestHandler<AddPizza, Unit>
{
    public async ValueTask<Unit> Handle(AddPizza request, CancellationToken ct)
    {
        var entity = new Pizza(request.Id, request.Name, request.Size, request.Price);
        await repo.AddAsync(entity, ct);

        var evt = new PizzaAddedEvent(entity.Id, entity.Name, entity.Size.ToString(), entity.Price);
        var messageKey = entity.Id.ToString();
        const string schemaId = "mm.pizzeria.pizza-added";
        var payload = JsonSerializer.Serialize(evt);

        var outboxMessage = new OutboxMessage(schemaId, messageKey, payload);
        await outboxRepo.AddAsync(outboxMessage, ct);

        await uow.SaveChangesAsync(ct);

        return Unit.Value;
    }
}