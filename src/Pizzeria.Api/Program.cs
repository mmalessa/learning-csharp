using System.Text.Json.Serialization;
using Mediator;
using Pizzeria.Application;
using Pizzeria.Application.Pizzas;
using Pizzeria.Domain;
using Pizzeria.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://+:80");

var connectionString = Environment.GetEnvironmentVariable("PIZZERIA_DB");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("PIZZERIA_DB environment variable must be set.");
}
Console.WriteLine($"Using connection string: {connectionString}");

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediator();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var pizzas = app.MapGroup("/pizza");

pizzas.MapGet("/", async (IMediator mediator) =>
{
    var items = await mediator.Send(new ListPizzas());
    return Results.Ok(items);
});

pizzas.MapPost("/", async (AddPizzaRequest request, IMediator mediator) =>
{
    try
    {
        await mediator.Send(new AddPizza(request.Id, request.Name, request.Size, request.Price));
        return Results.Created($"/pizza/{request.Id}", new
        {
            request.Id,
            request.Name,
            Size = request.Size,
            request.Price
        });
    }
    catch (ArgumentOutOfRangeException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

pizzas.MapDelete("/{id:int}", async (int id, IMediator mediator) =>
{
    var removed = await mediator.Send(new RemovePizza(id));
    return removed
        ? Results.Ok(new { removed = true })
        : Results.NotFound(new { error = $"Pizza {id} not found." });
});

app.MapGet("/", () => Results.Ok(new { service = "Pizzeria.Api", endpoints = "/pizza" }));

app.Run();

public sealed record AddPizzaRequest(int Id, string Name, PizzaSize Size, decimal Price);
