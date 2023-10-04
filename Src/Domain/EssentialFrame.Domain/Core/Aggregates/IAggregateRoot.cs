using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.Shared;
using EssentialFrame.Domain.Core.ValueObjects;
using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.Aggregates;

public interface IAggregateRoot<TAggregateIdentifier, TType> : IDeletableDomainObject
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    TAggregateIdentifier AggregateIdentifier { get; }

    TenantIdentifier TenantIdentifier { get; }

    IEnumerable<IDomainEvent<TAggregateIdentifier, TType>> GetUncommittedChanges();

    void ClearDomainEvents();
}