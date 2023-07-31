using System;
using System.Reflection;
using Bogus;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Factories;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Identity.Services;
using FluentAssertions;
using NUnit.Framework;

namespace EssentialFrame.Domain.Tests.UnitTests.Factories;

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
        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion);

        // Assert
        aggregate.Should().NotBeNull();
        aggregate.AggregateIdentifier.Should().Be(aggregateIdentifier);
        aggregate.AggregateVersion.Should().Be(aggregateVersion);
    }

    [Test]
    public void
        CreateAggregate_WhenAggregateIdentifierHasNotBeenProvided_ShouldThrowMissingAggregateIdentifierException()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.Empty;
        int aggregateVersion = _faker.Random.Int();

        // Act
        Action createAggregateAction = () =>
            GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion);

        // Assert
        createAggregateAction.Should().ThrowExactly<TargetInvocationException>()
            .WithInnerException<MissingAggregateIdentifierException>().WithMessage(
                $"The aggregate identifier is missing from the aggregate instance ({typeof(Post).FullName}).");
    }

    [Test]
    public void CreateAggregateWithIdentity_Always_ShouldCreateInstanceAndAssignValues()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        IdentityService identityService = new();

        // Act
        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion,
            identityService);

        // Assert
        aggregate.Should().NotBeNull();
        aggregate.AggregateIdentifier.Should().Be(aggregateIdentifier);
        aggregate.AggregateVersion.Should().Be(aggregateVersion);
        aggregate.GetIdentity().Should().BeEquivalentTo(identityService.GetCurrent());
    }

    [Test]
    public void
        CreateAggregateWithIdentity_WhenAggregateIdentifierHasNotBeenProvided_ShouldThrowMissingAggregateIdentifierException()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.Empty;
        int aggregateVersion = _faker.Random.Int();
        IdentityService identityService = new();

        // Act
        Action createAggregateAction = () =>
            GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion, identityService);

        // Assert
        createAggregateAction.Should().ThrowExactly<TargetInvocationException>()
            .WithInnerException<MissingAggregateIdentifierException>().WithMessage(
                $"The aggregate identifier is missing from the aggregate instance ({typeof(Post).FullName}).");
    }
}