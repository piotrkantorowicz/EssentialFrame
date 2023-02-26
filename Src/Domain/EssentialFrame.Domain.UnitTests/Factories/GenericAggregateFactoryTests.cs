using System;
using Bogus;
using EssentialFrame.Domain.Factories;
using EssentialFrame.TestData.Domain.Aggregates;
using EssentialFrame.TestData.Identity.Services;
using FluentAssertions;
using NUnit.Framework;

namespace EssentialFrame.Domain.UnitTests.Factories;

[TestFixture]
public sealed class GenericAggregateFactoryTests
{
    private readonly Faker _faker = new();

    [Test]
    public void CreateAggregate_Always_ShouldCreateInstanceAndAssignValues()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();

        // Act
        TestAggregate aggregate =
            GenericAggregateFactory<TestAggregate>.CreateAggregate(aggregateIdentifier, aggregateVersion);

        // Assert
        aggregate.Should().NotBeNull();
        aggregate.AggregateIdentifier.Should().Be(aggregateIdentifier);
        aggregate.AggregateVersion.Should().Be(aggregateVersion);
    }

    [Test]
    public void CreateAggregateWithIdentity_Always_ShouldCreateInstanceAndAssignValues()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        TestIdentityService identityService = new TestIdentityService();

        // Act
        TestAggregate aggregate =
            GenericAggregateFactory<TestAggregate>.CreateAggregate(aggregateIdentifier, aggregateVersion,
                identityService);

        // Assert
        aggregate.Should().NotBeNull();
        aggregate.AggregateIdentifier.Should().Be(aggregateIdentifier);
        aggregate.AggregateVersion.Should().Be(aggregateVersion);
        aggregate.GetIdentity().Should().BeEquivalentTo(identityService.GetCurrent());
    }
}