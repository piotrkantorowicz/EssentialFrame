using System;
using Bogus;
using EssentialFrame.Domain.Factories;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.Snapshots;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects;
using EssentialFrame.ExampleApp.Identity;
using EssentialFrame.Identity;
using EssentialFrame.Serialization.Interfaces;
using FluentAssertions;
using Moq;
using Moq.Language.Flow;
using NUnit.Framework;

namespace EssentialFrame.Domain.Tests.UnitTests.Snapshots;

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

        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new IdentityContext());

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion);
        Title expectedTitle = Title.Create(_faker.Lorem.Sentence(), true);
        string expectedDescription = _faker.Lorem.Sentences();
        DateTimeOffset expectedExpiration = _faker.Date.FutureOffset();
        IIdentityContext identityContext = _identityServiceMock.Object.GetCurrent();

        aggregate.ChangeTitle(expectedTitle, identityContext);
        aggregate.ChangeDescription(expectedDescription, identityContext);
        aggregate.ExtendExpirationDate(expectedExpiration, identityContext);

        ISetup<ISerializer, string> state = _serializerMock.Setup(s => s.Serialize(aggregate.State, null));

        // Act
        PostSnapshot snapshot = new(aggregateIdentifier, aggregateVersion, state);

        // Assert
        snapshot.AggregateIdentifier.Should().Be(aggregateIdentifier);
        snapshot.AggregateVersion.Should().Be(aggregateVersion);
        snapshot.AggregateState.Should().NotBeNull();
    }
}