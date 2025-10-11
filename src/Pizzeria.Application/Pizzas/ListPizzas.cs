using Mediator;
using Pizzeria.Application.Abstractions;
using Pizzeria.Domain;

namespace Pizzeria.Application.Pizzas;

public sealed record PizzaDto(int Id, string Name, PizzaSize Size, decimal Price);
public sealed record ListPizzas : IRequest<IReadOnlyList<PizzaDto>>;

public sealed class ListPizzasHandler(IPizzaRepository repo) : IRequestHandler<ListPizzas, IReadOnlyList<PizzaDto>>
{
    public async ValueTask<IReadOnlyList<PizzaDto>> Handle(ListPizzas request, CancellationToken ct)
    {
        var list = new List<PizzaDto>();
        await foreach (var p in repo.ListAsync(ct))
            list.Add(new PizzaDto(p.Id, p.Name, p.Size, p.Price));
        return list;
    }
}