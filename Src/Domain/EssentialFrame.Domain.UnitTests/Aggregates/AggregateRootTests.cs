using System;
using System.Collections.Generic;
using Bogus;
using EssentialFrame.Domain.Events;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Factories;
using EssentialFrame.Extensions;
using EssentialFrame.Serialization.Interfaces;
using EssentialFrame.TestData.Domain.Aggregates;
using EssentialFrame.TestData.Domain.DomainEvents;
using EssentialFrame.TestData.Domain.Entities;
using EssentialFrame.TestData.Domain.ValueObjects;
using EssentialFrame.TestData.Identity;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.UnitTests.Aggregates;

[TestFixture]
public sealed class AggregateRootTests
{
    private readonly Faker _faker = new();
    private readonly Mock<ISerializer> _serializerMock = new();

    [Test]
    public void CreateState_Always_ShouldReturnSpecificType()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();
        TestAggregate aggregate =
            GenericAggregateFactory<TestAggregate>.CreateAggregate(aggregateIdentifier, aggregateVersion);

        // Act
        TestAggregateState state = aggregate.CreateState();

        // Assert
        state.Should().BeOfType(typeof(TestAggregateState));
    }

    [Test]
    public void RestoreState_Always_ShouldAssignCorrectState()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();
        string serializedState = _faker.Lorem.Sentence();
        TestAggregate aggregate =
            GenericAggregateFactory<TestAggregate>.CreateAggregate(aggregateIdentifier, aggregateVersion);

        TestAggregateState expectedAggregateState = new(new TestTitle(_faker.Lorem.Sentence(), false),
            _faker.Lorem.Sentences(), _faker.Date.FutureOffset(),
            new HashSet<TestEntity> { new(_faker.Random.Guid(), _faker.Lorem.Word(), _faker.Random.Bytes(300)) });

        _serializerMock.Setup(s => s.Deserialize<TestAggregateState>(serializedState, typeof(TestAggregateState)))
            .Returns(expectedAggregateState);

        // Act
        aggregate.RestoreState(serializedState, _serializerMock.Object);

        // Assert
        aggregate.State.Should().BeOfType(typeof(TestAggregateState));

        TestAggregateState aggregateState = (TestAggregateState)aggregate.State;
        AssertState(aggregateState, expectedAggregateState);

        // Act
        aggregate.RestoreState(expectedAggregateState);

        // Assert
        AssertState(aggregateState, expectedAggregateState);
    }

    [Test]
    public void ApplyEvent_Always_ShouldModifyAggregateState()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;
        TestAggregate aggregate =
            GenericAggregateFactory<TestAggregate>.CreateAggregate(aggregateIdentifier, aggregateVersion);
        TestTitle expectedTitle = new(_faker.Lorem.Sentence(), true);
        string expectedDescription = _faker.Lorem.Sentences();
        DateTimeOffset expectedExpiration = _faker.Date.FutureOffset();

        // Act
        aggregate.ChangeTitle(expectedTitle);

        // Assert
        TestAggregateState aggregateState = (TestAggregateState)aggregate.State;
        aggregateState.Title.Should().Be(expectedTitle);

        // Act
        aggregate.ChangeDescription(expectedDescription);

        // Assert
        aggregateState = (TestAggregateState)aggregate.State;
        aggregateState.Description.Should().Be(expectedDescription);

        // Act
        aggregate.ExtendExpirationDate(expectedExpiration);

        // Assert
        aggregateState = (TestAggregateState)aggregate.State;
        aggregateState.Expiration.Should().Be(expectedExpiration);
    }

    [Test]
    public void GetUncommittedChanges_Always_ShouldGetAllChangesFromAggregate()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;
        TestAggregate aggregate =
            GenericAggregateFactory<TestAggregate>.CreateAggregate(aggregateIdentifier, aggregateVersion);
        TestTitle expectedTitle = new(_faker.Lorem.Sentence(), true);
        string expectedDescription = _faker.Lorem.Sentences();
        DateTimeOffset expectedExpiration = _faker.Date.FutureOffset();

        // Act
        aggregate.ChangeTitle(expectedTitle);
        aggregate.ChangeDescription(expectedDescription);
        aggregate.ExtendExpirationDate(expectedExpiration);

        // Assert
        IDomainEvent[] changes = aggregate.GetUncommittedChanges();
        changes.Should().HaveCount(3);
    }

    [Test]
    public void FlushUncommittedChanges_Always_ShouldIncreaseAggregateVersion()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;
        TestAggregate aggregate =
            GenericAggregateFactory<TestAggregate>.CreateAggregate(aggregateIdentifier, aggregateVersion);
        TestTitle expectedTitle = new(_faker.Lorem.Sentence(), true);
        string expectedDescription = _faker.Lorem.Sentences();
        DateTimeOffset expectedExpiration = _faker.Date.FutureOffset();

        // Act
        aggregate.ChangeTitle(expectedTitle);
        aggregate.ChangeDescription(expectedDescription);
        aggregate.ExtendExpirationDate(expectedExpiration);

        // Assert
        IDomainEvent[] changes = aggregate.FlushUncommittedChanges();
        changes.Should().HaveCount(3);
        aggregate.AggregateVersion.Should().Be(changes.Length);
        aggregate.GetUncommittedChanges().Should().BeEmpty();
    }

    [Test]
    public void Rehydrate_WhenHistoryIsValid_ShouldRestoreCorrectAggregate()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;
        TestIdentity identity = new();
        TestTitle title = new(_faker.Lorem.Sentence(), true);
        string description = _faker.Lorem.Sentences();
        DateTimeOffset expiration = _faker.Date.FutureOffset();
        Guid imageId = _faker.Random.Guid();
        string imageName = _faker.Lorem.Word();

        HashSet<TestEntity> images = new()
        {
            new(imageId, _faker.Lorem.Word(), _faker.Random.Bytes(200)),
            new(_faker.Random.Guid(), _faker.Lorem.Word(), _faker.Random.Bytes(500))
        };

        TestAggregate aggregate =
            GenericAggregateFactory<TestAggregate>.CreateAggregate(aggregateIdentifier, aggregateVersion);

        ChangeTitleTestDomainEvent changeTitleEvent = new(aggregateIdentifier, identity, aggregateVersion + 1, title);

        ChangeDescriptionTestDomainEvent changeDescription =
            new(aggregateIdentifier, identity, aggregateVersion + 2, description);

        ChangeExpirationDateTestDomainEvent changeExpirationEvent =
            new(aggregateIdentifier, identity, aggregateVersion + 3, expiration);

        AddImagesDomainEvent addImagesEvent = new(aggregateIdentifier, identity, aggregateVersion + 4, images);

        ChangeImageNameDomainEvent changeImageNameEvent =
            new(aggregateIdentifier, identity, aggregateVersion + 5, imageId, imageName);

        IDomainEvent[] history =
        {
            changeTitleEvent, changeDescription, changeExpirationEvent, addImagesEvent, changeImageNameEvent
        };

        // Act
        aggregate.Rehydrate(history);

        // Assert
        aggregate.AggregateVersion.Should().Be(history.Length);
    }

    [Test]
    public void Rehydrate_WhenHistoryIsUnordered_ShouldThrowUnorderedEventsException()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;
        TestIdentity identity = new();
        TestTitle title = new(_faker.Lorem.Sentence(), true);
        DateTimeOffset expiration = _faker.Date.FutureOffset();
        TestAggregate aggregate =
            GenericAggregateFactory<TestAggregate>.CreateAggregate(aggregateIdentifier, aggregateVersion);
        ChangeTitleTestDomainEvent changeTitleEvent = new(aggregateIdentifier, identity, aggregateVersion + 2, title);
        ChangeExpirationDateTestDomainEvent changeExpirationEvent =
            new(aggregateIdentifier, identity, aggregateVersion + 1, expiration);
        IDomainEvent[] history = { changeTitleEvent, changeExpirationEvent };

        // Act
        Action action = () => aggregate.Rehydrate(history);

        // Assert
        action.Should().ThrowExactly<UnorderedEventsException>()
            .WithMessage($"The events for this aggregate are not in the expected order ({aggregateIdentifier}).");
    }

    [Test]
    public void Rehydrate_WhenOneOfEventsHaveUnMatchedAggregateId_ShouldThrowUnmatchedDomainEventException()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;
        TestIdentity identity = new();
        DateTimeOffset expiration = _faker.Date.FutureOffset();
        TestAggregate aggregate =
            GenericAggregateFactory<TestAggregate>.CreateAggregate(aggregateIdentifier, aggregateVersion);
        ChangeExpirationDateTestDomainEvent changeExpirationEvent =
            new(_faker.Random.Guid(), identity, aggregateVersion + 2, expiration);
        IDomainEvent[] history = { changeExpirationEvent };

        // Act
        Action action = () => aggregate.Rehydrate(history);

        // Assert
        action.Should().ThrowExactly<UnmatchedDomainEventException>().WithMessage(
            $"Aggregate ({aggregate.GetTypeFullName()}) with identifier: ({aggregateIdentifier}) doesn't match " +
            $"provided domain event ({changeExpirationEvent.GetTypeFullName()}) with expected aggregate identifier: ({changeExpirationEvent.AggregateIdentifier}).");
    }

    private static void AssertState(TestAggregateState aggregateState, TestAggregateState expectedAggregateState)
    {
        aggregateState.Title.Should().Be(expectedAggregateState.Title);
        aggregateState.Description.Should().Be(expectedAggregateState.Description);
        aggregateState.Expiration.Should().Be(expectedAggregateState.Expiration);
        aggregateState.Images.Should().HaveCount(1);
    }
}