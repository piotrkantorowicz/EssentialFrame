using System;
using Bogus;
using EssentialFrame.Domain.Factories;
using EssentialFrame.ExampleApp.Application.Identity;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.Identity;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.Tests.UnitTests.Factories;

[TestFixture]
public sealed class GenericAggregateFactoryTests
{
    private readonly Faker _faker = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();

    [SetUp]
    public void SetUp()
    {
        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new IdentityContext());
    }

    [TearDown]
    public void TearDown()
    {
        _identityServiceMock.Reset();
    }
    
    [Test]
    public void CreateAggregate_NonParameters_ShouldCreateInstanceAndAssignValues()
    {
        // Arrange
        // Act
        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(_identityServiceMock.Object.GetCurrent());

        // Assert
        aggregate.Should().NotBeNull();
        aggregate.AggregateIdentifier.Should().NotBeEmpty();
        aggregate.AggregateVersion.Should().Be(0);
        aggregate.IdentityContext.Should().BeEquivalentTo(_identityServiceMock.Object.GetCurrent());
    }

    [Test]
    public void CreateAggregate_WithIdentifier_ShouldCreateInstanceAndAssignValues()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();

        // Act
        Post aggregate =
            GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
                _identityServiceMock.Object.GetCurrent());

        // Assert
        aggregate.Should().NotBeNull();
        aggregate.AggregateIdentifier.Should().Be(aggregateIdentifier);
        aggregate.AggregateVersion.Should().Be(0);
        aggregate.IdentityContext.Should().BeEquivalentTo(_identityServiceMock.Object.GetCurrent());
    }
    
    [Test]
    public void CreateAggregate_WithVersion_ShouldCreateInstanceAndAssignValues()
    {
        // Arrange
        int aggregateVersion = _faker.Random.Int();

        // Act
        Post aggregate =
            GenericAggregateFactory<Post>.CreateAggregate(aggregateVersion, _identityServiceMock.Object.GetCurrent());

        // Assert
        aggregate.Should().NotBeNull();
        aggregate.AggregateIdentifier.Should().NotBeEmpty();
        aggregate.AggregateVersion.Should().Be(aggregateVersion);
        aggregate.IdentityContext.Should().BeEquivalentTo(_identityServiceMock.Object.GetCurrent());
    }

    [Test]
    public void CreateAggregate_WithIdentifierAndVersion_ShouldCreateInstanceAndAssignValues()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();

        // Act
        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion,
            _identityServiceMock.Object.GetCurrent());

        // Assert
        aggregate.Should().NotBeNull();
        aggregate.AggregateIdentifier.Should().Be(aggregateIdentifier);
        aggregate.AggregateVersion.Should().Be(aggregateVersion);
        aggregate.IdentityContext.Should().BeEquivalentTo(_identityServiceMock.Object.GetCurrent());
    }
}