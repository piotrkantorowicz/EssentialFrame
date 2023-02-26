using System;
using Bogus;
using EssentialFrame.Domain.Factories;
using EssentialFrame.Identity;
using EssentialFrame.Serialization.Interfaces;
using EssentialFrame.TestData.Domain.Aggregates;
using EssentialFrame.TestData.Domain.Snapshots;
using EssentialFrame.TestData.Domain.ValueObjects;
using EssentialFrame.TestData.Identity;
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
    private readonly Mock<IIdentityService> _identityServiceMock = new();

    [Test]
    public void CreateSnapshotInstance_Always_AssignCorrectValues()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;

        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new TestIdentity());

        TestAggregate aggregate = GenericAggregateFactory<TestAggregate>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);
        
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