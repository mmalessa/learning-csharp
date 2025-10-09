using LearningCSharp.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningCSharp.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        // Klucz główny
        builder.HasKey(o => o.Id);

        // Właściwości
        builder.Property(o => o.Id)
            .ValueGeneratedNever(); // ponieważ tworzysz Id w domenie, nie w DB

        builder.Property(o => o.CustomerId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Total)
            .HasPrecision(18, 2); // precyzja dla wartości pieniężnych

        builder.Property(o => o.CreatedAt)
            .HasColumnType("timestamp without time zone") // PostgreSQL-friendly
            .IsRequired();

        // (opcjonalnie) jeśli chcesz indeksy
        builder.HasIndex(o => o.CustomerId)
            .HasDatabaseName("idx_orders_customer");

        // Możesz dodać domyślne sortowanie, filtrowanie itp.
    }
}