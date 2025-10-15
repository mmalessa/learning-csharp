using System;
using System.Text.Json.Serialization;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Pizzeria.Application;
using Pizzeria.Domain;
using Pizzeria.Infrastructure;

namespace Pizzeria.Api.Configuration;

internal static class ServiceConfigurationExtensions
{
    public static void ConfigurePizzeriaServices(this WebApplicationBuilder builder)
    {
        ConfigureJsonSerialization(builder.Services);
        ConfigureSwagger(builder.Services);

        builder.Services.AddMediator();
        builder.Services.AddApplication();

        var connectionString = GetRequiredSetting(builder.Configuration, "PIZZERIA_DB");
        Console.WriteLine($"Using connection string: {connectionString}");
        builder.Services.AddInfrastructure(connectionString);

        var kafkaBootstrapServers = GetRequiredSetting(builder.Configuration, "KAFKA_BOOTSTRAP_SERVERS");
        var kafkaTopic = GetRequiredSetting(builder.Configuration, "KAFKA_TOPIC");
        builder.Services.AddOutboxProcessor(kafkaBootstrapServers, kafkaTopic);
    }

    private static void ConfigureJsonSerialization(IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }

    private static void ConfigureSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Pizzeria API", Version = "v1" });

            options.MapType<PizzaSize>(() => new OpenApiSchema
            {
                Type = "string",
                Enum = Enum.GetNames<PizzaSize>()
                    .Select<string, IOpenApiAny>(name => new OpenApiString(name))
                    .ToList()
            });
        });
    }

    private static string GetRequiredSetting(IConfiguration configuration, string key)
    {
        var value = configuration[key];
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{key} environment variable must be set.");
        }

        return value;
    }
}
