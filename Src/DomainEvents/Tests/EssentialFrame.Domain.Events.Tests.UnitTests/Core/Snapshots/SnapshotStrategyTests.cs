using Bogus;
using EssentialFrame.Domain.Events.Core.Factories;
using EssentialFrame.Domain.Events.Core.Snapshots;
using EssentialFrame.ExampleApp.Application.Identity;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;
using EssentialFrame.Identity;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.Events.Tests.UnitTests.Core.Snapshots;

[TestFixture]
public class SnapshotStrategyTests
{
    private const int Interval = 5;
    private readonly Faker _faker = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();

    private Post _aggregate;

    [SetUp]
    public void Setup()
    {
        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new IdentityContext());

        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 1;

        _aggregate =
            GenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier, aggregateVersion);

        _aggregate.Create(Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
            Date.Create(_faker.Date.FutureOffset()), null, _identityServiceMock.Object.GetCurrent());
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
        _aggregate.ChangeTitle(Title.Default(_faker.Lorem.Sentence()), _identityServiceMock.Object.GetCurrent());
        _aggregate.ChangeTitle(Title.Default(_faker.Lorem.Sentence()), _identityServiceMock.Object.GetCurrent());
        _aggregate.ChangeDescription(Description.Create(_faker.Lorem.Sentences()),
            _identityServiceMock.Object.GetCurrent());

        // Act
        bool result = new SnapshotStrategy<Post, PostIdentifier>(Interval).ShouldTakeSnapShot(_aggregate);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void ShouldTakeSnapShot_WhenAggregateVersionIsBelowInterval_ReturnsFalse()
    {
        // Arrange
        _aggregate.ChangeTitle(Title.Default(_faker.Lorem.Sentence()), _identityServiceMock.Object.GetCurrent());
        _aggregate.ChangeDescription(Description.Create(_faker.Lorem.Sentences()),
            _identityServiceMock.Object.GetCurrent());

        // Act
        bool result = new SnapshotStrategy<Post, PostIdentifier>(Interval).ShouldTakeSnapShot(_aggregate);

        // Assert
        result.Should().BeFalse();
    }
}