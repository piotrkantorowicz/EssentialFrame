using System;
using Bogus;
using EssentialFrame.Domain.Events.Core.Factories;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using FluentAssertions;
using NUnit.Framework;

namespace EssentialFrame.Domain.Events.Tests.UnitTests.Persistence.Aggregates;

[TestFixture]
public class AggregateMapperTests
{
    private readonly Faker _faker = new();
    private readonly IAggregateMapper _aggregateMapper = new AggregateMapper();

    [Test]
    public void MapToAggregateDataModel_WhenCalledWithValidAggregateRoot_ReturnsAggregateDataModel()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Int();
        Guid? tenantIdentifier = _faker.Random.Guid();

        Post aggregateRoot =
            GenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier, aggregateVersion,
                tenantIdentifier);

        // Act
        AggregateDataModel aggregateDataModel = _aggregateMapper.Map(aggregateRoot);

        // Assert
        aggregateDataModel.Should().NotBeNull();
        aggregateDataModel.AggregateIdentifier.Should().Be(aggregateIdentifier.Identifier);
        aggregateDataModel.AggregateVersion.Should().Be(aggregateVersion);
        aggregateDataModel.TenantIdentifier.Should().Be(tenantIdentifier);
    }
}