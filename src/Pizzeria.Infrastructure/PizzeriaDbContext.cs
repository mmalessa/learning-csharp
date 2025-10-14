using Microsoft.EntityFrameworkCore;
using Pizzeria.Domain;

namespace Pizzeria.Infrastructure;

public sealed class PizzeriaDbContext(DbContextOptions<PizzeriaDbContext> options) : DbContext(options)
{
    public DbSet<Pizza> Pizzas => Set<Pizza>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

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

        b.Entity<OutboxMessage>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SchemaId).IsRequired().HasMaxLength(200);
            e.Property(x => x.MessageKey).IsRequired().HasMaxLength(200);
            e.Property(x => x.Payload).IsRequired();
            e.Property(x => x.CreatedAt).IsRequired();
            e.Property(x => x.ProcessedAt);
            e.Property(x => x.Error);
            e.HasIndex(x => new { x.ProcessedAt, x.CreatedAt });
        });
    }
}