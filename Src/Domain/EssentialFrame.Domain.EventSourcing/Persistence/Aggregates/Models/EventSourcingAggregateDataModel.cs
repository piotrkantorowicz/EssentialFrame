namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;

public class EventSourcingAggregateDataModel
{
    public Guid AggregateIdentifier { get; set; }

    public Guid? TenantIdentifier { get; set; }

    public int AggregateVersion { get; set; }

    public DateTimeOffset? DeletedDate { get; set; }

    public bool IsDeleted { get; set; }
}