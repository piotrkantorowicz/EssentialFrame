﻿using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.ValueObjects;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.EventSourcing.Core.Aggregates;

public interface IEventSourcingAggregateRoot<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    TAggregateIdentifier AggregateIdentifier { get; }

    TenantIdentifier TenantIdentifier { get; }

    int AggregateVersion { get; }

    IDomainEvent<TAggregateIdentifier, TType>[] GetUncommittedChanges();

    EventSourcingAggregateState<TAggregateIdentifier, TType> State { get; }

    IDomainEvent<TAggregateIdentifier, TType>[] FlushUncommittedChanges();

    void Rehydrate(IEnumerable<IDomainEvent<TAggregateIdentifier, TType>> history);

    void RestoreState(object aggregateState, int version);

    void RestoreState(string aggregateStateString, int version, ISerializer serializer);
}