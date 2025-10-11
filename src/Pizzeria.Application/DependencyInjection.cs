using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Pizzeria.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // services.AddMediatR(cfg => 
        //     // cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly)
        //     cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
        //     );
        return services;
    }
}