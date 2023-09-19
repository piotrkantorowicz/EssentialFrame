using EssentialFrame.Domain.Core.Events;
using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.Shared;
using EssentialFrame.Domain.Core.ValueObjects;
using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.Aggregates;

public interface IAggregateRoot<TAggregateIdentifier> : IDeletableDomainObject
    where TAggregateIdentifier : TypedGuidIdentifier
{
    TAggregateIdentifier AggregateIdentifier { get; }

    TenantIdentifier TenantIdentifier { get; }

    IDomainEvent<TAggregateIdentifier>[]
        FlushUncommittedChanges(IDomainEventsPublisher<TAggregateIdentifier> publisher);
}