namespace Pizzeria.Domain;

public class OutboxMessage
{
    public Guid Id { get; private set; }
    public string SchemaId { get; private set; } = null!;
    public string MessageKey { get; private set; } = null!;
    public string Payload { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? Error { get; private set; }

    private OutboxMessage() {} // EF

    public OutboxMessage(string schemaId, string messageKey, string payload)
    {
        if (string.IsNullOrWhiteSpace(schemaId))
            throw new ArgumentException("SchemaId required", nameof(schemaId));
        if (string.IsNullOrWhiteSpace(messageKey))
            throw new ArgumentException("MessageKey required", nameof(messageKey));
        if (string.IsNullOrWhiteSpace(payload))
            throw new ArgumentException("Payload required", nameof(payload));

        Id = Guid.NewGuid();
        SchemaId = schemaId;
        MessageKey = messageKey;
        Payload = payload;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsProcessed()
    {
        ProcessedAt = DateTime.UtcNow;
        Error = null;
    }

    public void MarkAsFailed(string error)
    {
        Error = error ?? throw new ArgumentNullException(nameof(error));
    }
}
