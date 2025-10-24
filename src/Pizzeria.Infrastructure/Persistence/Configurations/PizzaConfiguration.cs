using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pizzeria.Domain;

namespace Pizzeria.Infrastructure.Persistence.Configurations;

internal sealed class PizzaConfiguration : IEntityTypeConfiguration<Pizza>
{
    public void Configure(EntityTypeBuilder<Pizza> e)
    {
        e.HasKey(x => x.Id); // Klucz główny
        e.Property(x => x.Id).ValueGeneratedNever();
        e.Property(x => x.Name).IsRequired().HasMaxLength(200);
        e.Property(x => x.Price).HasColumnType("decimal(10,2)");
        e.Property(x => x.Size).HasConversion<int>();
    }
}