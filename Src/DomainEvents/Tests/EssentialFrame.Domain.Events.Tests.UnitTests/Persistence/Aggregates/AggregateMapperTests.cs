using System;
using Bogus;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;
using EssentialFrame.Domain.Factories;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Identity;
using EssentialFrame.Identity;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.Events.Tests.UnitTests.Persistence.Aggregates;

[TestFixture]
public class AggregateMapperTests
{
    private readonly Faker _faker = new();
    private readonly IAggregateMapper _aggregateMapper = new AggregateMapper();
    private readonly Mock<IIdentityService> _identityServiceMock = new();

    [SetUp]
    public void Setup()
    {
        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new IdentityContext());
    }

    [TearDown]
    public void TearDown()
    {
        _identityServiceMock.Reset();
    }

    [Test]
    public void MapToAggregateDataModel_WhenCalledWithValidAggregateRoot_ReturnsAggregateDataModel()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();

        Post aggregateRoot =
            GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion,
                _identityServiceMock.Object.GetCurrent());

        // Act
        AggregateDataModel aggregateDataModel = _aggregateMapper.Map(aggregateRoot);

        // Assert
        aggregateDataModel.Should().NotBeNull();
        aggregateDataModel.AggregateIdentifier.Should().Be(aggregateIdentifier);
        aggregateDataModel.AggregateVersion.Should().Be(aggregateVersion);
        aggregateDataModel.TenantIdentifier.Should().Be(_identityServiceMock.Object.GetCurrent().Tenant.Identifier);
    }
}