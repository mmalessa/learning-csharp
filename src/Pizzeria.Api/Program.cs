using Pizzeria.Api.Configuration;
using Pizzeria.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://+:80");

builder.ConfigurePizzeriaServices();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPizzeriaEndpoints();

app.Run();
