using System;
using System.Threading.Tasks;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Pizzeria.Application.Pizzas;
using Pizzeria.Domain;

namespace Pizzeria.Api.Endpoints;

internal static class PizzeriaEndpoints
{
    public static void MapPizzeriaEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Ok(new { service = "Pizzeria.Api", endpoints = "/pizzas" }));

        var pizzas = app.MapGroup("/pizzas");

        pizzas.MapGet("/", ListPizzasAsync)
            .WithName("ListPizzas");

        pizzas.MapPost("/", AddPizzaAsync)
            .WithName("AddPizza");

        pizzas.MapDelete("/{id:int}", RemovePizzaAsync)
            .WithName("RemovePizza");
    }

    private static async Task<IResult> ListPizzasAsync(IMediator mediator)
    {
        var items = await mediator.Send(new ListPizzas());
        return Results.Ok(items);
    }

    private static async Task<IResult> AddPizzaAsync(AddPizzaDto dto, IMediator mediator)
    {
        try
        {
            await mediator.Send(new AddPizza(dto.Id, dto.Name, dto.Size, dto.Price));
            return Results.Created($"/pizzas/{dto.Id}", dto);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> RemovePizzaAsync(int id, IMediator mediator)
    {
        var removed = await mediator.Send(new RemovePizza(id));
        return removed
            ? Results.Ok(new { removed = true })
            : Results.NotFound(new { error = $"Pizza {id} not found." });
    }
}

internal sealed record AddPizzaDto(int Id, string Name, PizzaSize Size, decimal Price);
