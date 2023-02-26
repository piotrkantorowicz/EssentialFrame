using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Events;
using EssentialFrame.Identity;
using EssentialFrame.TestData.Domain.Entities;

namespace EssentialFrame.TestData.Domain.DomainEvents;

public class AddImagesDomainEvent : DomainEventBase
{
    public AddImagesDomainEvent(Guid aggregateIdentifier, IIdentity identity, HashSet<TestEntity> newImages) : base(
        aggregateIdentifier, identity)
    {
        NewImages = newImages;
    }

    public AddImagesDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity,
        HashSet<TestEntity> newImages) : base(aggregateIdentifier, eventIdentifier, identity)
    {
        NewImages = newImages;
    }

    public AddImagesDomainEvent(Guid aggregateIdentifier, IIdentity identity, int expectedVersion,
        HashSet<TestEntity> newImages) : base(aggregateIdentifier, identity, expectedVersion)
    {
        NewImages = newImages;
    }

    public AddImagesDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity, int expectedVersion,
        HashSet<TestEntity> newImages) : base(aggregateIdentifier, eventIdentifier, identity, expectedVersion)
    {
        NewImages = newImages;
    }

    public HashSet<TestEntity> NewImages { get; }
}