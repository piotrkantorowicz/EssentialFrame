using Bogus;
using EssentialFrame.Domain.EventSourcing.Core.Factories;
using EssentialFrame.ExampleApp.Application.Identity;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.Snapshots;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;
using EssentialFrame.Identity.Interfaces;
using EssentialFrame.Serialization.Interfaces;
using FluentAssertions;
using Moq;
using Moq.Language.Flow;
using NUnit.Framework;

namespace EssentialFrame.Domain.EventSourcing.Tests.UnitTests.Core.Snapshots;

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
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        ;
        const int aggregateVersion = 0;

        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new AppIdentityContext());

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        Title expectedTitle = Title.Default(_faker.Lorem.Sentence());
        Description expectedDescription = Description.Create(_faker.Lorem.Sentences());
        Date expectedExpiration = Date.Create(_faker.Date.FutureOffset());

        aggregate.Create(Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
            Date.Create(_faker.Date.FutureOffset()), null, _identityServiceMock.Object.GetCurrent());

        aggregate.ChangeTitle(expectedTitle, _identityServiceMock.Object.GetCurrent());
        aggregate.ChangeDescription(expectedDescription, _identityServiceMock.Object.GetCurrent());
        aggregate.ExtendExpirationDate(expectedExpiration, _identityServiceMock.Object.GetCurrent());

        ISetup<ISerializer, string> state = _serializerMock.Setup(s => s.Serialize(aggregate.State));

        // Act
        PostSnapshot snapshot = new(aggregateIdentifier, aggregateVersion, state);

        // Assert
        snapshot.AggregateIdentifier.Should().Be(aggregateIdentifier);
        snapshot.AggregateVersion.Should().Be(aggregateVersion);
        snapshot.AggregateState.Should().NotBeNull();
    }
}