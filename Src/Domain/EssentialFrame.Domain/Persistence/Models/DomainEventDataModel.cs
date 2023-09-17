namespace EssentialFrame.Domain.Persistence.Models;

public class DomainEventDataModel
{
    public virtual Guid AggregateIdentifier { get; set; }

    public virtual int AggregateVersion { get; set; }

    public virtual Guid EventIdentifier { get; set; }

    public virtual object DomainEvent { get; set; }

    public virtual string EventClass { get; set; }

    public virtual string EventType { get; set; }

    public virtual DateTimeOffset? CreatedAt { get; set; }
}