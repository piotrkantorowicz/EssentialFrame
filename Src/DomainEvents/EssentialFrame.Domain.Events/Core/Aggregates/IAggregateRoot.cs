using EssentialFrame.Identity;

namespace EssentialFrame.Domain.Events.Core.Aggregates;

public interface IAggregateRoot
{
    public Guid AggregateIdentifier { get; }

    public int AggregateVersion { get; }

    public IIdentityContext IdentityContext { get; }

    public AggregateState State { get; }

    public IDomainEvent[] GetUncommittedChanges();

    public IDomainEvent[] FlushUncommittedChanges();

    public void Rehydrate(IEnumerable<IDomainEvent> history);
}