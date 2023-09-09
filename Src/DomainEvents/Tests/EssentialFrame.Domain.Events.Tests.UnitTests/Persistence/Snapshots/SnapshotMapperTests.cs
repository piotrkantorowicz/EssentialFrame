using System.Collections.Generic;
using Bogus;
using EssentialFrame.Domain.Events.Core.Snapshots;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Mappers;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Models;
using EssentialFrame.ExampleApp.Application.Identity;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.Identity;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.Events.Tests.UnitTests.Persistence.Snapshots;

[TestFixture]
public class SnapshotMapperTests
{
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

    private readonly Faker _faker = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();

    [Test]
    public void Map_Always_ShouldMapSnapshotToSnapshotDataModel()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Int();
        string serializedState = _faker.Random.String(_faker.Random.Int(100, 300));
        Snapshot<PostIdentifier> snapshot = new(aggregateIdentifier, aggregateVersion, serializedState);
        SnapshotMapper<PostIdentifier> mapper = new();

        // Act
        SnapshotDataModel result = mapper.Map(snapshot);

        // Assert
        AssertSnapshotDataModel(result, aggregateIdentifier, aggregateVersion, serializedState);
    }

    [Test]
    public void Map_Always_ShouldMapSnapshotDataModelToSnapshot()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Int();
        string serializedState = _faker.Random.String(_faker.Random.Int(100, 300));
        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = aggregateIdentifier.Identifier,
            AggregateVersion = aggregateVersion,
            AggregateState = serializedState
        };
        SnapshotMapper<PostIdentifier> mapper = new();

        // Act
        Snapshot<PostIdentifier> result = mapper.Map(snapshotDataModel);

        // Assert
        AssertSnapshot(result, aggregateIdentifier, aggregateVersion, serializedState);
    }

    [Test]
    public void Map_Always_ShouldMapCollectionOfSnapshots()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Int();
        string serializedState = _faker.Random.String(_faker.Random.Int(100, 300));
        Snapshot<PostIdentifier> snapshot = new(aggregateIdentifier, aggregateVersion, serializedState);
        Snapshot<PostIdentifier>[] snapshots = { snapshot };
        SnapshotMapper<PostIdentifier> snapshotMapper = new();

        // Act
        IReadOnlyCollection<SnapshotDataModel> result = snapshotMapper.Map(snapshots);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(snapshots.Length);

        foreach (SnapshotDataModel snapshotDataModel in result)
        {
            AssertSnapshotDataModel(snapshotDataModel, aggregateIdentifier, aggregateVersion, serializedState);
        }
    }

    [Test]
    public void Map_Always_ShouldMapCollectionOfSnapshotDataModels()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Int();
        string serializedState = _faker.Random.String(_faker.Random.Int(100, 300));
        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = aggregateIdentifier.Identifier,
            AggregateVersion = aggregateVersion,
            AggregateState = serializedState
        };
        SnapshotDataModel[] snapshotDataModels = { snapshotDataModel };
        SnapshotMapper<PostIdentifier> mapper = new();

        // Act
        IReadOnlyCollection<Snapshot<PostIdentifier>> result = mapper.Map(snapshotDataModels);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(snapshotDataModels.Length);

        foreach (Snapshot<PostIdentifier> snapshot in result)
        {
            AssertSnapshot(snapshot, aggregateIdentifier, aggregateVersion, serializedState);
        }
    }

    private void AssertSnapshot(Snapshot<PostIdentifier> snapshot, PostIdentifier aggregateIdentifier,
        int aggregateVersion, string serializedState)
    {
        snapshot.AggregateIdentifier.Identifier.Should().Be(aggregateIdentifier.Identifier);
        snapshot.AggregateVersion.Should().Be(aggregateVersion);
        snapshot.AggregateState.Should().Be(serializedState);
    }

    private void AssertSnapshotDataModel(SnapshotDataModel snapshotDataModel, PostIdentifier aggregateIdentifier,
        int aggregateVersion, string serializedState)
    {
        snapshotDataModel.AggregateIdentifier.Should().Be(aggregateIdentifier.Identifier);
        snapshotDataModel.AggregateVersion.Should().Be(aggregateVersion);
        snapshotDataModel.AggregateState.Should().Be(serializedState);
    }
}