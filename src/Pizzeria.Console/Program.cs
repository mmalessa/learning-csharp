using Microsoft.Extensions.Hosting;
using Pizzeria.Console.Configuration;
using Pizzeria.Console;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigurePizzeriaServices();

using var host = builder.Build();

var app = CommandAppFactory.Create(host.Services);

return app.Run(args);
