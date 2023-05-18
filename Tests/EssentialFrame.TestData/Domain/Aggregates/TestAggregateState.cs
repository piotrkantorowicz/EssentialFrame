using System;
using System.Collections.Generic;
using System.Linq;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.TestData.Domain.DomainEvents;
using EssentialFrame.TestData.Domain.Entities;
using EssentialFrame.TestData.Domain.Rules;
using EssentialFrame.TestData.Domain.ValueObjects;

namespace EssentialFrame.TestData.Domain.Aggregates;

public sealed class TestAggregateState : AggregateState
{
    private readonly Guid _aggregateIdentifier;
    private readonly Type _aggregateType;

    private TestAggregateState(Guid aggregateId, Type aggregateType)
    {
        _aggregateIdentifier = aggregateId;
        _aggregateType = aggregateType;

        Images = new HashSet<TestEntity>();
    }

    private TestAggregateState(Guid aggregateId, Type aggregateType, TestTitle title, string description,
        DateTimeOffset expiration, HashSet<TestEntity> images) : this(aggregateId, aggregateType)
    {
        Title = title;
        Description = description;
        Expiration = expiration;
        Images = images;
    }

    public TestTitle Title { get; private set; }

    public string Description { get; private set; }

    public DateTimeOffset Expiration { get; private set; }

    public HashSet<TestEntity> Images { get; }

    internal static TestAggregateState Create(Guid aggregateId, Type aggregateType, TestTitle title, string description,
        DateTimeOffset expiration, HashSet<TestEntity> images)
    {
        TestAggregateState aggregateState = new(aggregateId, aggregateType, title, description, expiration, images);

        aggregateState.CheckRule(new CannotCreateOutdatedAggregateRule(aggregateId, aggregateType, expiration));

        return aggregateState;
    }

    internal static TestAggregateState Create(Guid aggregateId, Type aggregateType)
    {
        TestAggregateState aggregateState = new(aggregateId, aggregateType);

        return aggregateState;
    }

    public void When(ChangeTitleTestDomainEvent @event)
    {
        CheckRule(new ExpiredTestAggregateCannotBeUpdatedRule(_aggregateIdentifier, _aggregateType, Expiration));

        Title = @event.NewTitle;
    }

    public void When(ChangeDescriptionTestDomainEvent @event)
    {
        CheckRule(new ExpiredTestAggregateCannotBeUpdatedRule(_aggregateIdentifier, _aggregateType, Expiration));

        Description = @event.NewDescription;
    }

    public void When(ChangeExpirationDateTestDomainEvent @event)
    {
        CheckRule(new ExpiredTestAggregateCannotBeUpdatedRule(_aggregateIdentifier, _aggregateType,
            @event.NewExpirationDate));

        Expiration = @event.NewExpirationDate;
    }

    public void When(AddImagesDomainEvent @event)
    {
        CheckRule(new ExpiredTestAggregateCannotBeUpdatedRule(_aggregateIdentifier, _aggregateType, Expiration));

        foreach (TestEntity newImage in @event.NewImages)
        {
            CheckRule(new TestEntityNameMustBeUniqueWithInAggregateRule(_aggregateIdentifier, _aggregateType,
                newImage.Name, Images.Select(i => i.Name).ToArray()));
        }

        HashSet<TestEntity> images = @event.NewImages;

        foreach (TestEntity image in images)
        {
            Images.Add(image);
        }
    }

    public void When(ChangeImageNameDomainEvent @event)
    {
        CheckRule(new ExpiredTestAggregateCannotBeUpdatedRule(_aggregateIdentifier, _aggregateType, Expiration));

        CheckRule(new TestEntityNameMustBeUniqueWithInAggregateRule(_aggregateIdentifier, _aggregateType,
            @event.NewImageName, Images.Select(i => i.Name).ToArray()));

        TestEntity image = Images.FirstOrDefault(i => i.EntityIdentifier == @event.ImageId);

        image?.UpdateName(@event.NewImageName);
    }
}