using System.Text.Json.Serialization;
using Mediator;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Pizzeria.Application;
using Pizzeria.Application.Abstractions;
using Pizzeria.Application.Pizzas;
using Pizzeria.Domain;
using Pizzeria.Infrastructure;
using Pizzeria.Infrastructure.Kafka;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://+:80");

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Pizzeria API", Version = "v1" });

        // Enum jako string
        options.MapType<PizzaSize>(() => new OpenApiSchema
        {
            Type = "string",
            Enum = Enum.GetNames<PizzaSize>()
                .Select<string, IOpenApiAny> (name => new OpenApiString(name))
                .ToList()
        });
    }
);

builder.Services.AddMediator();
builder.Services.AddApplication();

var connectionString = Environment.GetEnvironmentVariable("PIZZERIA_DB")
    ?? throw new InvalidOperationException("PIZZERIA_DB environment variable must be set.");

Console.WriteLine($"Using connection string: {connectionString}");
builder.Services.AddInfrastructure(connectionString);

var kafkaBs = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS")
    ?? throw new InvalidOperationException("KAFKA_BOOTSTRAP_SERVERS environment variable must be set.");
var kafkaTopic = Environment.GetEnvironmentVariable("KAFKA_TOPIC")
    ?? throw new InvalidOperationException("KAFKA_TOPIC environment variable must be set.");

builder.Services.AddSingleton<IEventDispatcher>(_ =>
    new KafkaEventDispatcher(kafkaBs, kafkaTopic)
);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var pizzas = app.MapGroup("/pizzas");

pizzas.MapGet("/", async (IMediator mediator) =>
{
    var items = await mediator.Send(new ListPizzas());
    return Results.Ok(items);
})
.WithName("ListPizzas");

pizzas.MapPost("/", async (AddPizzaDto dto, IMediator mediator) =>
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
})
.WithName("AddPizza");

pizzas.MapDelete("/{id:int}", async (int id, IMediator mediator) =>
{
    var removed = await mediator.Send(new RemovePizza(id));
    return removed
        ? Results.Ok(new { removed = true })
        : Results.NotFound(new { error = $"Pizza {id} not found." });
})
.WithName("RemovePizza");

app.MapGet("/", () => Results.Ok(new { service = "Pizzeria.Api", endpoints = "/pizzas" }));

app.Run();

public sealed record AddPizzaDto(int Id, string Name, PizzaSize Size, decimal Price);
