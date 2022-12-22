namespace EssentialFrame.Domain.Aggregates;

public class SerializedAggregate
{
    public string AggregateClass { get; set; }

    public DateTimeOffset? AggregateExpires { get; set; }

    public Guid AggregateIdentifier { get; set; }

    public string AggregateType { get; set; }

    public Guid TenantIdentifier { get; set; }
}


