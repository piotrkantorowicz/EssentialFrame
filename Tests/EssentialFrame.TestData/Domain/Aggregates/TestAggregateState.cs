using System;
using System.Collections.Generic;
using System.Linq;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.TestData.Domain.DomainEvents;
using EssentialFrame.TestData.Domain.Entities;
using EssentialFrame.TestData.Domain.ValueObjects;

namespace EssentialFrame.TestData.Domain.Aggregates;

public class TestAggregateState : AggregateState
{
    public TestAggregateState()
    {
        Images = new HashSet<TestEntity>();
    }

    public TestAggregateState(TestTitle title, string description, DateTimeOffset expiration,
        HashSet<TestEntity> images)
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

    public void When(ChangeTitleTestDomainEvent @event)
    {
        Title = @event.NewTitle;
    }

    public void When(ChangeDescriptionTestDomainEvent @event)
    {
        Description = @event.NewDescription;
    }

    public void When(ChangeExpirationDateTestDomainEvent @event)
    {
        Expiration = @event.NewExpirationDate;
    }

    public void When(AddImagesDomainEvent @event)
    {
        HashSet<TestEntity> images = @event.NewImages;

        foreach (TestEntity image in images)
        {
            Images.Add(image);
        }
    }

    public void When(ChangeImageNameDomainEvent @event)
    {
        TestEntity image = Images.FirstOrDefault(i => i.EntityIdentifier == @event.ImageId);

        image?.UpdateName(@event.NewImageName);
    }
}