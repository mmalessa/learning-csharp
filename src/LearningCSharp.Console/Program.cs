using Microsoft.Extensions.Hosting;
using LearningCSharp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Pobieramy connection string ze zmiennej środowiskowej
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Console.WriteLine("ERROR: ConnectionStrings__Default not set in environment variables.");
            Environment.Exit(1);
        }

        // Rejestracja DbContext z providerem PostgreSQL
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Tutaj możesz dodać repozytoria lub inne serwisy
        // services.AddScoped<IOrderRepository, OrderRepository>();
    })
    .Build();
//
// Tworzymy scope, pobieramy DbContext i wykonujemy migracje
using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

Console.WriteLine("Applying migrations...");
db.Database.Migrate();
Console.WriteLine("Migrations applied successfully!");
