using System;
using Bogus;
using EssentialFrame.Domain.Events.Core.Snapshots;
using EssentialFrame.Domain.Factories;
using EssentialFrame.ExampleApp.Application.Identity;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;
using EssentialFrame.Identity;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.Events.Tests.UnitTests.Core.Snapshots;

[TestFixture]
public class SnapshotStrategyTests
{
    private const int Interval = 4;
    private readonly Faker _faker = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();

    [SetUp]
    public void Setup()
    {
        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new IdentityContext());
    }

    [TearDown]
    public void TearDown()
    {
        _identityServiceMock.Reset();
    }

    [Test]
    public void ShouldTakeSnapShot_WhenAggregateVersionIsOverOfInterval_ReturnsTrue()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 1;

        Post aggregate =
            GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion,
                _identityServiceMock.Object);

        aggregate.ChangeTitle(Title.Default(_faker.Lorem.Sentence()));
        aggregate.ChangeTitle(Title.Default(_faker.Lorem.Sentence()));
        aggregate.ChangeDescription(Description.Create(_faker.Lorem.Sentences()));

        // Act
        bool result = new SnapshotStrategy(Interval).ShouldTakeSnapShot(aggregate);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void ShouldTakeSnapShot_WhenAggregateVersionIsBelowInterval_ReturnsFalse()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 1;

        Post aggregate =
            GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion,
                _identityServiceMock.Object);

        aggregate.ChangeTitle(Title.Default(_faker.Lorem.Sentence()));
        aggregate.ChangeDescription(Description.Create(_faker.Lorem.Sentences()));

        // Act
        bool result = new SnapshotStrategy(Interval).ShouldTakeSnapShot(aggregate);

        // Assert
        result.Should().BeFalse();
    }
}