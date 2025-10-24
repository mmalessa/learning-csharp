using Microsoft.EntityFrameworkCore;
using Pizzeria.Domain;

namespace Pizzeria.Infrastructure;

public sealed class PizzeriaDbContext(DbContextOptions<PizzeriaDbContext> options) : DbContext(options)
{
    public DbSet<Pizza> Pizzas => Set<Pizza>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // Zastosuj wszystkie konfiguracje z tego assembly (patrz: pliki Configuration)
        b.ApplyConfigurationsFromAssembly(typeof(PizzeriaDbContext).Assembly);
        // skanuje całe wskazane assembly (czyli projekt, w którym znajduje się PizzeriaDbContext)
        // i automatycznie wyszukuje wszystkie klasy, które implementują IEntityTypeConfiguration<T>.
    }
}