using Microsoft.EntityFrameworkCore;
using Pizzeria.Domain;

namespace Pizzeria.Infrastructure;

public sealed class PizzeriaDbContext(DbContextOptions<PizzeriaDbContext> options) : DbContext(options)
{
    public DbSet<Pizza> Pizzas => Set<Pizza>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Pizza>(e =>
        {
            e.HasKey(x => x.Id); // Klucz główny
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Price).HasColumnType("decimal(10,2)");
            e.Property(x => x.Size).HasConversion<int>();
        });
    }
}