using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Identity;
using EssentialFrame.Serialization.Interfaces;
using EssentialFrame.TestData.Domain.DomainEvents;
using EssentialFrame.TestData.Domain.Entities;
using EssentialFrame.TestData.Domain.ValueObjects;

namespace EssentialFrame.TestData.Domain.Aggregates;

public sealed class TestAggregate : AggregateRoot
{
    private TestAggregate(Guid aggregateIdentifier, int aggregateVersion) : base(aggregateIdentifier, aggregateVersion)
    {
    }

    private TestAggregate(Guid aggregateIdentifier, int aggregateVersion, IIdentityService identityService) : base(
        aggregateIdentifier, aggregateVersion, identityService)
    {
    }

    public override TestAggregateState CreateState()
    {
        return TestAggregateState.Create(AggregateIdentifier, GetType());
    }

    public TestAggregateState CreateState(TestTitle title, string description, DateTimeOffset expiration,
        HashSet<TestEntity> images)
    {
        return TestAggregateState.Create(AggregateIdentifier, GetType(), title, description, expiration, images);
    }

    public override void RestoreState(object aggregateState, ISerializer serializer = null)
    {
        switch (aggregateState)
        {
            case null:
                return;
            case string serializedState:
                State = serializer?.Deserialize<TestAggregateState>(serializedState, typeof(TestAggregateState));
                return;
            default:
                State = (TestAggregateState)aggregateState;
                break;
        }
    }

    public void ChangeTitle(TestTitle title)
    {
        ChangeTitleTestDomainEvent @event = new(AggregateIdentifier, GetIdentity(), title);
        Apply(@event);
    }

    public void ChangeDescription(string description)
    {
        ChangeDescriptionTestDomainEvent @event = new(AggregateIdentifier, GetIdentity(), description);
        Apply(@event);
    }

    public void ExtendExpirationDate(DateTimeOffset newExpirationDate)
    {
        ChangeExpirationDateTestDomainEvent @event = new(AggregateIdentifier, GetIdentity(), newExpirationDate);
        Apply(@event);
    }

    public void AddImages(HashSet<TestEntity> images)
    {
        AddImagesDomainEvent @event = new(AggregateIdentifier, GetIdentity(), images);
        Apply(@event);
    }

    public void ChangeImageName(Guid imageId, string name)
    {
        ChangeImageNameDomainEvent @event = new(AggregateIdentifier, GetIdentity(), imageId, name);
        Apply(@event);
    }
}