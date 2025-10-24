using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pizzeria.Domain;

namespace Pizzeria.Infrastructure.Persistence.Configurations;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.SchemaId).IsRequired().HasMaxLength(200);
        e.Property(x => x.MessageKey).IsRequired().HasMaxLength(200);
        e.Property(x => x.Payload).IsRequired();
        e.Property(x => x.CreatedAt).IsRequired();
        e.Property(x => x.ProcessedAt);
        e.Property(x => x.Error);
        e.HasIndex(x => new { x.ProcessedAt, x.CreatedAt });
    }
}