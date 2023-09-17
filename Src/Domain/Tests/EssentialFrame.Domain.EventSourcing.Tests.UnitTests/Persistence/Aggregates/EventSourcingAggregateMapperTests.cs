using Bogus;
using EssentialFrame.Domain.Core.ValueObjects;
using EssentialFrame.Domain.EventSourcing.Core.Factories;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Mappers;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using FluentAssertions;
using NUnit.Framework;

namespace EssentialFrame.Domain.EventSourcing.Tests.UnitTests.Persistence.Aggregates;

[TestFixture]
public class EventSourcingAggregateMapperTests
{
    private readonly Faker _faker = new();

    private readonly IEventSourcingAggregateMapper<PostIdentifier> _eventSourcingAggregateMapper =
        new EventSourcingAggregateMapper<PostIdentifier>();

    [Test]
    public void MapToAggregateDataModel_WhenCalledWithValidAggregateRoot_ReturnsAggregateDataModel()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Int();
        TenantIdentifier tenantIdentifier = TenantIdentifier.New(_faker.Random.Guid());

        Post aggregateRoot =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion, tenantIdentifier);

        // Act
        EventSourcingAggregateDataModel eventSourcingAggregateDataModel =
            _eventSourcingAggregateMapper.Map(aggregateRoot);

        // Assert
        eventSourcingAggregateDataModel.Should().NotBeNull();
        eventSourcingAggregateDataModel.AggregateIdentifier.Should().Be(aggregateIdentifier.Value);
        eventSourcingAggregateDataModel.AggregateVersion.Should().Be(aggregateVersion);
        eventSourcingAggregateDataModel.TenantIdentifier.Should().Be(tenantIdentifier.Value);
    }
}