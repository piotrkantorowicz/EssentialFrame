using Bogus;
using EssentialFrame.Domain.Core.ValueObjects;
using EssentialFrame.Domain.EventSourcing.Core.Factories;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using FluentAssertions;
using NUnit.Framework;

namespace EssentialFrame.Domain.EventSourcing.Tests.UnitTests.Core.Factories;

[TestFixture]
public sealed class GenericEventSourcingAggregateFactoryTests
{
    private readonly Faker _faker = new();

    [Test]
    public void CreateAggregate_WithIdentifier_ShouldCreateInstanceAndAssignValues()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());

        // Act
        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier);

        // Assert
        aggregate.Should().NotBeNull();
        aggregate.AggregateIdentifier.Should().Be(aggregateIdentifier);
    }

    [Test]
    public void CreateAggregate_WithIdentifierAndVersionAndIdentity_ShouldCreateInstanceAndAssignValues()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Int();

        // Act
        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        // Assert
        aggregate.Should().NotBeNull();
        aggregate.AggregateIdentifier.Should().Be(aggregateIdentifier);
        aggregate.AggregateVersion.Should().Be(aggregateVersion);
    }

    [Test]
    public void CreateAggregate_WithIdentifierAndVersionAndIdentityAndTenant_ShouldCreateInstanceAndAssignValues()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Int();
        TenantIdentifier tenantIdentifier = TenantIdentifier.New(_faker.Random.Guid());

        // Act
        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion, tenantIdentifier);

        // Assert
        aggregate.Should().NotBeNull();
        aggregate.AggregateIdentifier.Should().Be(aggregateIdentifier);
        aggregate.AggregateVersion.Should().Be(aggregateVersion);
        aggregate.TenantIdentifier.Should().Be(tenantIdentifier);
    }
}