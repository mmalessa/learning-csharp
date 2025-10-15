using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pizzeria.Application.Abstractions;
using Pizzeria.Infrastructure.Outbox;
using Pizzeria.Infrastructure.Persistence.Repositories;

namespace Pizzeria.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string? connectionString)
    {
        services.AddDbContext<PizzeriaDbContext>(opt => opt.UseSqlServer(connectionString));
        services.AddScoped<IPizzaRepository, PizzaRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }

    public static IServiceCollection AddOutboxProcessor(
        this IServiceCollection services,
        string bootstrapServers,
        string topic,
        TimeSpan? interval = null,
        int batchSize = 100)
    {
        services.AddSingleton<IOutboxBatchProcessor>(sp =>
        {
            var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
            var logger = sp.GetRequiredService<ILogger<OutboxBatchProcessor>>();
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                Acks = Acks.All
            };
            return new OutboxBatchProcessor(scopeFactory, config, topic, logger);
        });

        services.AddHostedService(sp =>
        {
            var batchProcessor = sp.GetRequiredService<IOutboxBatchProcessor>();
            var logger = sp.GetRequiredService<ILogger<OutboxProcessor>>();
            return new OutboxProcessor(batchProcessor, logger, interval, batchSize);
        });
        return services;
    }
}
