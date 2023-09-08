using System;
using Bogus;
using EssentialFrame.Domain.Events.Core.Factories;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using FluentAssertions;
using NUnit.Framework;

namespace EssentialFrame.Domain.Events.Tests.UnitTests.Core.Factories;

[TestFixture]
public sealed class GenericAggregateFactoryTests
{
    private readonly Faker _faker = new();

    [Test]
    public void CreateAggregate_WithIdentifier_ShouldCreateInstanceAndAssignValues() 
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        
        // Act
        Post aggregate = GenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier);

        // Assert
        aggregate.Should().NotBeNull();
        aggregate.AggregateIdentifier.Should().Be(aggregateIdentifier);
    }
    
    [Test]
    public void CreateAggregate_WithIdentifierAndVersionAndIdentity_ShouldCreateInstanceAndAssignValues()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        
        // Act
        Post aggregate =
            GenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier, aggregateVersion);

        // Assert
        aggregate.Should().NotBeNull();
        aggregate.AggregateIdentifier.Should().Be(aggregateIdentifier);
        aggregate.AggregateVersion.Should().Be(aggregateVersion);
    }

    [Test]
    public void CreateAggregate_WithIdentifierAndVersionAndIdentityAndTenant_ShouldCreateInstanceAndAssignValues()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        Guid? tenantIdentifier = _faker.Random.Guid();

        // Act
        Post aggregate =
            GenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier, aggregateVersion,
                tenantIdentifier);

        // Assert
        aggregate.Should().NotBeNull();
        aggregate.AggregateIdentifier.Should().Be(aggregateIdentifier);
        aggregate.AggregateVersion.Should().Be(aggregateVersion);
        aggregate.TenantIdentifier.Should().Be(tenantIdentifier);
    }
}