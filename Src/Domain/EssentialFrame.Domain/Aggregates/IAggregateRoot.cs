using EssentialFrame.Domain.Events;
using EssentialFrame.Domain.Shared;
using EssentialFrame.Domain.ValueObjects;
using EssentialFrame.Domain.ValueObjects.Core;

namespace EssentialFrame.Domain.Aggregates;

public interface IAggregateRoot<TAggregateIdentifier> : IDeletableDomainObject
    where TAggregateIdentifier : TypedGuidIdentifier
{
    TAggregateIdentifier AggregateIdentifier { get; }

    TenantIdentifier TenantIdentifier { get; }

    IDomainEvent<TAggregateIdentifier>[] GetUncommittedChanges();
}