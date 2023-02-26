using System;
using Bogus;
using EssentialFrame.Domain.Factories;
using EssentialFrame.Serialization.Interfaces;
using EssentialFrame.TestData.Domain.Aggregates;
using EssentialFrame.TestData.Domain.Snapshots;
using EssentialFrame.TestData.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Moq.Language.Flow;
using NUnit.Framework;

namespace EssentialFrame.Domain.UnitTests.Snapshots;

[TestFixture]
public class SnapshotTests
{
    private readonly Faker _faker = new();
    private readonly Mock<ISerializer> _serializerMock = new();

    [Test]
    public void CreateSnapshotInstance_Always_AssignCorrectValues()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;
        TestAggregate aggregate =
            GenericAggregateFactory<TestAggregate>.CreateAggregate(aggregateIdentifier, aggregateVersion);
        TestTitle expectedTitle = new(_faker.Lorem.Sentence(), true);
        string expectedDescription = _faker.Lorem.Sentences();
        DateTimeOffset expectedExpiration = _faker.Date.FutureOffset();

        aggregate.ChangeTitle(expectedTitle);
        aggregate.ChangeDescription(expectedDescription);
        aggregate.ExtendExpirationDate(expectedExpiration);

        ISetup<ISerializer, string> state = _serializerMock.Setup(s => s.Serialize(aggregate.State));

        // Act
        TestSnapshot snapshot = new(aggregateIdentifier, aggregateVersion, state);

        // Assert
        snapshot.AggregateIdentifier.Should().Be(aggregateIdentifier);
        snapshot.AggregateVersion.Should().Be(aggregateVersion);
        snapshot.AggregateState.Should().NotBeNull();
    }
}