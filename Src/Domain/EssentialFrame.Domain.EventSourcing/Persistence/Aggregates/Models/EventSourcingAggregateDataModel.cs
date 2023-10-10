namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;

public class EventSourcingAggregateDataModel
{
    public virtual string AggregateIdentifier { get; set; }

    public virtual Guid? TenantIdentifier { get; set; }

    public virtual int AggregateVersion { get; set; }
}