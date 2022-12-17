namespace EssentialFrame.Outbox;

public class OutboxMessage
{
    private OutboxMessage()
    {
    }

    public OutboxMessage(Guid id,
                         DateTime occurredOn,
                         string type,
                         string data)
    {
        Id = id;
        OccurredOn = occurredOn;
        Type = type;
        Data = data;
    }

    public Guid Id { get; }

    public DateTime OccurredOn { get; }

    public string Type { get; }

    public string Data { get; }

    public DateTime? ProcessedDate { get; }
}

