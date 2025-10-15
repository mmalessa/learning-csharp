using System;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pizzeria.Application;
using Pizzeria.Application.Abstractions;
using Pizzeria.Console.Commands;
using Pizzeria.Infrastructure;
using Pizzeria.Infrastructure.Kafka;

namespace Pizzeria.Console.Configuration;

internal static class HostBuilderExtensions
{
    public static IHostBuilder ConfigurePizzeriaServices(this IHostBuilder builder)
    {
        return builder.ConfigureServices((_, services) =>
        {
            services.AddMediator();
            services.AddApplication();

            var connectionString = GetRequiredEnvironmentVariable("PIZZERIA_DB");
            services.AddInfrastructure(connectionString);

            var kafkaBootstrapServers = GetRequiredEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS");
            var kafkaTopic = GetRequiredEnvironmentVariable("KAFKA_TOPIC");

            services.AddOutboxProcessor(kafkaBootstrapServers, kafkaTopic);
            services.AddSingleton<IEventDispatcher>(_ => new KafkaEventDispatcher(kafkaBootstrapServers, kafkaTopic));

            services.AddTransient<AddPizzaCommand>();
            services.AddTransient<RemovePizzaCommand>();
            services.AddTransient<ListPizzasCommand>();
            services.AddTransient<OutboxConsumeCommand>();
        });
    }

    private static string GetRequiredEnvironmentVariable(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{key} environment variable must be set.");
        }

        return value;
    }
}
