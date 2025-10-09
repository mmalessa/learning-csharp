using LearningCSharp.Api.Orders;
using LearningCSharp.Application.Orders;
using LearningCSharp.Domain.Orders;
using LearningCSharp.Infrastructure.Persistence;
using LearningCSharp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register Dependency Injection
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<CreateOrderHandler>();

// EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConnectionStrings__Default")));

// Swagger / Open API
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => new { message = "Elo, elo!" });

app.MapPost("/api/orders", async (CreateOrderRequest request, CreateOrderHandler handler, CancellationToken ct) =>
{
   
    await handler.HandleAsync(
        request.OrderId,
        request.CustomerId,
        request.Total,
        DateTime.UtcNow,
        ct
    );

    return Results.StatusCode(StatusCodes.Status201Created);
});

app.Run();
