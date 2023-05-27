using System;
using System.Collections.Generic;
using Bogus;
using EssentialFrame.Domain.Events;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Identity;
using EssentialFrame.TestData.Domain.DomainEvents;
using EssentialFrame.TestData.Domain.Entities;
using EssentialFrame.TestData.Domain.ValueObjects;
using EssentialFrame.TestData.Identity;
using EssentialFrame.Time;
using FluentAssertions;
using NUnit.Framework;

namespace EssentialFrame.Domain.UnitTests.Events;

[TestFixture]
public sealed class DomainEventTests
{
    private readonly Faker _faker = new();

    [Test]
    public void CreateChangeTitleDomainEvent_Always_ShouldAssignCorrectValues()
    {
        // Arrange
        TestIdentity identity = new();
        Guid aggregateIdentifier = _faker.Random.Guid();
        Guid eventIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();
        TestTitle title = TestTitle.Create(_faker.Lorem.Sentence(), true);

        // Act
        ChangeTitleTestDomainEvent @event = new(aggregateIdentifier, identity, title);

        // Assert
        AssertIdentity(identity, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewTitle.Should().Be(title);

        // Act
        @event = new ChangeTitleTestDomainEvent(aggregateIdentifier, eventIdentifier, identity, title);

        // Assert
        AssertIdentity(identity, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewTitle.Should().Be(title);

        // Act
        @event = new ChangeTitleTestDomainEvent(aggregateIdentifier, identity, aggregateVersion, title);

        // Assert
        AssertIdentity(identity, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewTitle.Should().Be(title);

        // Act
        @event = new ChangeTitleTestDomainEvent(aggregateIdentifier, eventIdentifier, identity, aggregateVersion,
            title);

        // Assert
        AssertIdentity(identity, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewTitle.Should().Be(title);
    }

    [Test]
    public void CreateChangeDescriptionDomainEvent_Always_ShouldAssignCorrectValues()
    {
        // Arrange
        TestIdentity identity = new();
        Guid aggregateIdentifier = _faker.Random.Guid();
        Guid eventIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();
        string description = _faker.Lorem.Sentences();

        // Act
        ChangeDescriptionTestDomainEvent @event = new(aggregateIdentifier, identity, description);

        // Assert
        AssertIdentity(identity, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewDescription.Should().Be(description);

        // Act
        @event = new ChangeDescriptionTestDomainEvent(aggregateIdentifier, eventIdentifier, identity, description);

        // Assert
        AssertIdentity(identity, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewDescription.Should().Be(description);

        // Act
        @event = new ChangeDescriptionTestDomainEvent(aggregateIdentifier, identity, aggregateVersion, description);

        // Assert
        AssertIdentity(identity, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewDescription.Should().Be(description);

        // Act
        @event = new ChangeDescriptionTestDomainEvent(aggregateIdentifier, eventIdentifier, identity, aggregateVersion,
            description);

        // Assert
        AssertIdentity(identity, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewDescription.Should().Be(description);
    }

    [Test]
    public void CreateChangeExpirationDateDomainEvent_Always_ShouldAssignCorrectValues()
    {
        // Arrange
        TestIdentity identity = new();
        Guid aggregateIdentifier = _faker.Random.Guid();
        Guid eventIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();
        DateTimeOffset expirationOffset = _faker.Date.FutureOffset();

        // Act
        ChangeExpirationDateTestDomainEvent @event = new(aggregateIdentifier, identity, expirationOffset);

        // Assert
        AssertIdentity(identity, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewExpirationDate.Should().Be(expirationOffset);

        // Act
        @event = new ChangeExpirationDateTestDomainEvent(aggregateIdentifier, eventIdentifier, identity,
            expirationOffset);

        // Assert
        AssertIdentity(identity, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewExpirationDate.Should().Be(expirationOffset);

        // Act
        @event = new ChangeExpirationDateTestDomainEvent(aggregateIdentifier, identity, aggregateVersion,
            expirationOffset);

        // Assert
        AssertIdentity(identity, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewExpirationDate.Should().Be(expirationOffset);

        // Act
        @event = new ChangeExpirationDateTestDomainEvent(aggregateIdentifier, eventIdentifier, identity,
            aggregateVersion, expirationOffset);

        // Assert
        AssertIdentity(identity, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewExpirationDate.Should().Be(expirationOffset);
    }

    [Test]
    public void AddImagesDomainEvent_Always_ShouldAssignCorrectValues()
    {
        // Arrange
        TestIdentity identity = new();
        Guid aggregateIdentifier = _faker.Random.Guid();
        Guid eventIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();
        HashSet<TestEntity> images = new()
        {
            TestEntity.Create(_faker.Random.Guid(), _faker.Lorem.Word(), _faker.Random.Bytes(200)),
            TestEntity.Create(_faker.Random.Guid(), _faker.Lorem.Word(), _faker.Random.Bytes(500))
        };

        // Act
        AddImagesDomainEvent @event = new(aggregateIdentifier, identity, images);

        // Assert
        AssertIdentity(identity, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewImages.Should().BeSameAs(images);

        // Act
        @event = new AddImagesDomainEvent(aggregateIdentifier, eventIdentifier, identity, images);

        // Assert
        AssertIdentity(identity, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewImages.Should().BeSameAs(images);

        // Act
        @event = new AddImagesDomainEvent(aggregateIdentifier, identity, aggregateVersion, images);

        // Assert
        AssertIdentity(identity, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewImages.Should().BeSameAs(images);

        // Act
        @event = new AddImagesDomainEvent(aggregateIdentifier, eventIdentifier, identity, aggregateVersion, images);

        // Assert
        AssertIdentity(identity, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewImages.Should().BeSameAs(images);
    }

    [Test]
    public void ChangeImageNameDomainEvent_Always_ShouldAssignCorrectValues()
    {
        // Arrange
        TestIdentity identity = new();
        Guid aggregateIdentifier = _faker.Random.Guid();
        Guid eventIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();
        Guid imageId = _faker.Random.Guid();
        string imageName = _faker.Lorem.Word();

        // Act
        ChangeImageNameDomainEvent @event = new(aggregateIdentifier, identity, imageId, imageName);

        // Assert
        AssertIdentity(identity, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.ImageId.Should().Be(imageId);
        @event.NewImageName.Should().Be(imageName);

        // Act
        @event = new ChangeImageNameDomainEvent(aggregateIdentifier, eventIdentifier, identity, imageId, imageName);

        // Assert
        AssertIdentity(identity, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.ImageId.Should().Be(imageId);
        @event.NewImageName.Should().Be(imageName);

        // Act
        @event = new ChangeImageNameDomainEvent(aggregateIdentifier, identity, aggregateVersion, imageId, imageName);

        // Assert
        AssertIdentity(identity, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.ImageId.Should().Be(imageId);
        @event.NewImageName.Should().Be(imageName);

        // Act
        @event = new ChangeImageNameDomainEvent(aggregateIdentifier, eventIdentifier, identity, aggregateVersion,
            imageId, imageName);

        // Assert
        AssertIdentity(identity, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.ImageId.Should().Be(imageId);
        @event.NewImageName.Should().Be(imageName);
    }

    [Test]
    public void AdjustAggregateVersion_OnValidAggregate_ShouldAssignCorrectAggregateProperties()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();

        ChangeTitleTestDomainEvent @event = new(aggregateIdentifier, new TestIdentity(),
            TestTitle.Create(_faker.Lorem.Sentence(), true));

        // Act
        @event.AdjustAggregateVersion(aggregateIdentifier, aggregateVersion);

        // Assert
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.EventTime.Should().BeCloseTo(SystemClock.UtcNow, TimeSpan.FromMilliseconds(100));
    }

    [Test]
    public void AdjustAggregateVersion_OnInvalidAggregate_ShouldThrowDomainEventDoesNotMatchException()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();

        ChangeTitleTestDomainEvent @event = new(_faker.Random.Guid(), new TestIdentity(),
            TestTitle.Create(_faker.Lorem.Sentence(), true));

        // Act
        Action action = () => @event.AdjustAggregateVersion(aggregateIdentifier, aggregateVersion);

        // Assert
        action.Should().ThrowExactly<DomainEventDoesNotMatchException>().WithMessage(
            $"The event is not match aggregate ({aggregateIdentifier}), because was assigned to other aggregate ({@event.AggregateIdentifier}).");
    }

    private static void AssertIdentity(IIdentity provided, IDomainEvent expected)
    {
        provided.User.Identifier.Should().Be(expected.UserIdentity);
        provided.Service.GetFullIdentifier().Should().Be(expected.ServiceIdentity);
        provided.Tenant.Identifier.Should().Be(expected.TenantIdentity);
        provided.Correlation.Identifier.Should().Be(expected.CorrelationIdentity);
    }
}