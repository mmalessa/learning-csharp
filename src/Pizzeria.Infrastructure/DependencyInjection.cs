using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pizzeria.Application.Abstractions;
using Pizzeria.Infrastructure.Persistence.Repositories;

namespace Pizzeria.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string? connectionString)
    {
        services.AddDbContext<PizzeriaDbContext>(opt => opt.UseSqlServer(connectionString));
        services.AddScoped<IPizzaRepository, PizzaRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}