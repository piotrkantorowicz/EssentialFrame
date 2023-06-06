using System;
using System.Collections.Generic;
using Bogus;
using EssentialFrame.Domain.Events.Persistence.Snapshots;
using EssentialFrame.Domain.Snapshots;
using EssentialFrame.ExampleApp.Identity;
using EssentialFrame.Identity;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.Events.Tests.UnitTests.Snapshots;

[TestFixture]
public class SnapshotMapperTests
{
    private readonly Faker _faker = new();
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
    public void Map_Always_ShouldMapSnapshotToSnapshotDataModel()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        string serializedState = _faker.Random.String(_faker.Random.Int(100, 300));
        Snapshot snapshot = new(aggregateIdentifier, aggregateVersion, serializedState);
        SnapshotMapper mapper = new();

        // Act
        SnapshotDataModel result = mapper.Map(snapshot);

        // Assert
        result.Should().BeEquivalentTo(snapshot);
    }

    [Test]
    public void Map_Always_ShouldMapSnapshotDataModelToSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        string serializedState = _faker.Random.String(_faker.Random.Int(100, 300));
        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = aggregateIdentifier,
            AggregateVersion = aggregateVersion,
            AggregateState = serializedState
        };
        SnapshotMapper mapper = new();

        // Act
        Snapshot result = mapper.Map(snapshotDataModel);

        // Assert
        result.Should().BeEquivalentTo(snapshotDataModel);
    }

    [Test]
    public void Map_Always_ShouldMapCollectionOfSnapshots()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        string serializedState = _faker.Random.String(_faker.Random.Int(100, 300));
        Snapshot snapshot = new(aggregateIdentifier, aggregateVersion, serializedState);
        Snapshot[] snapshots = new[] { snapshot };
        SnapshotMapper snapshotMapper = new();

        // Act
        IReadOnlyCollection<SnapshotDataModel> result = snapshotMapper.Map(snapshots);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(snapshots.Length);
        result.Should().BeEquivalentTo(snapshots);
    }

    [Test]
    public void Map_Always_ShouldMapCollectionOfSnapshotDataModels()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        string serializedState = _faker.Random.String(_faker.Random.Int(100, 300));
        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = aggregateIdentifier,
            AggregateVersion = aggregateVersion,
            AggregateState = serializedState
        };
        SnapshotDataModel[] snapshotDataModels = new[] { snapshotDataModel };
        SnapshotMapper mapper = new();

        // Act
        IReadOnlyCollection<Snapshot> result = mapper.Map(snapshotDataModels);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(snapshotDataModels.Length);
        result.Should().BeEquivalentTo(snapshotDataModels);
    }
}