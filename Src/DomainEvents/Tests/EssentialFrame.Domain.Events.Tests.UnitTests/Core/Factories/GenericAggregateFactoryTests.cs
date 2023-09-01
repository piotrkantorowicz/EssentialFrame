using System;
using System.Reflection;
using Bogus;
using EssentialFrame.Domain.Events.Core.Factories;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.ExampleApp.Application.Identity;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.Identity;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.Events.Tests.UnitTests.Core.Factories;

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
    public void CreateAggregate_WithIdentifierAndIdentity_ShouldCreateInstanceAndAssignValues()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();

        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new IdentityContext());

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
    public void CreateAggregate_WithVersionAndIdentity_ShouldCreateInstanceAndAssignValues()
    {
        // Arrange
        int aggregateVersion = _faker.Random.Int();

        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new IdentityContext());

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
    public void CreateAggregate_WithIdentifierAndVersionAndIdentity_ShouldCreateInstanceAndAssignValues()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();

        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new IdentityContext());

        // Act
        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion,
            _identityServiceMock.Object.GetCurrent());

        // Assert
        aggregate.Should().NotBeNull();
        aggregate.AggregateIdentifier.Should().Be(aggregateIdentifier);
        aggregate.AggregateVersion.Should().Be(aggregateVersion);
        aggregate.IdentityContext.Should().BeEquivalentTo(_identityServiceMock.Object.GetCurrent());
    }

    [Test]
    public void CreateAggregate_WithIdentifierButWithoutIdentity_ShouldThrowMissingIdentityContextException()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();

        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns((IdentityContext)null);

        // Act
        Action createAggregateAction = () =>
            GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
                _identityServiceMock.Object.GetCurrent());

        // Assert
        createAggregateAction.Should().ThrowExactly<TargetInvocationException>()
            .WithInnerException<MissingIdentityContextException>().WithMessage(
                $"This aggregate ({typeof(Post).FullName}) has missing identity context. Consider to build your aggregates via constructor allows to pass an identity parameter");
    }

    [Test]
    public void CreateAggregate_WithVersionButWithoutIdentity_ShouldThrowMissingIdentityContextException()
    {
        // Arrange
        int aggregateVersion = _faker.Random.Int();

        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns((IdentityContext)null);

        // Act
        Action createAggregateAction = () =>
            GenericAggregateFactory<Post>.CreateAggregate(aggregateVersion, _identityServiceMock.Object.GetCurrent());

        // Assert
        createAggregateAction.Should().ThrowExactly<TargetInvocationException>()
            .WithInnerException<MissingIdentityContextException>().WithMessage(
                $"This aggregate ({typeof(Post).FullName}) has missing identity context. Consider to build your aggregates via constructor allows to pass an identity parameter");
    }

    [Test]
    public void CreateAggregate_WithIdentifierAndVersionButWithoutIdentity_ShouldThrowMissingIdentityContextException()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();

        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns((IdentityContext)null);

        // Act
        Action createAggregateAction = () =>
            GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion,
                _identityServiceMock.Object.GetCurrent());

        // Assert
        createAggregateAction.Should().ThrowExactly<TargetInvocationException>()
            .WithInnerException<MissingIdentityContextException>().WithMessage(
                $"This aggregate ({typeof(Post).FullName}) has missing identity context. Consider to build your aggregates via constructor allows to pass an identity parameter");
    }
}