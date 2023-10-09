using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.EventSourcing.Exceptions;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Models;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Services;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Services.Interfaces;
using EssentialFrame.ExampleApp.Application.Identity;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.Identity.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.EventSourcing.Tests.UnitTests.Persistence.Snapshots;

[TestFixture]
public class DefaultSnapshotStoreTests
{
    private readonly Faker _faker = new();
    private readonly CancellationToken _cancellationToken = default;
    
    private readonly IList<Encoding> _encodings = new List<Encoding>
    {
        Encoding.Default,
        Encoding.Unicode,
        Encoding.UTF8,
        Encoding.UTF32,
        Encoding.ASCII
    };
    
    private Mock<ICache<string, SnapshotDataModel>> _snapshotCacheMock;
    private Mock<IIdentityService> _identityServiceMock;
    private Mock<ISnapshotOfflineStorage> _snapshotOfflineStorageMock;
    private Encoding _encoding;
    
    [SetUp]
    public void SetUp()
    {
        _encoding = _faker.Random.ListItem(_encodings);
        _snapshotCacheMock = new Mock<ICache<string, SnapshotDataModel>>();
        _identityServiceMock = new Mock<IIdentityService>();
        _snapshotOfflineStorageMock = new Mock<ISnapshotOfflineStorage>();

        _identityServiceMock.Setup(x => x.GetCurrent()).Returns(new AppIdentityContext());
    }

    [TearDown]
    public void TearDown()
    {
        _encoding = null;
        
        _snapshotCacheMock.Reset();
        _identityServiceMock.Reset();
        _snapshotOfflineStorageMock.Reset();
    }

    [Test]
    public void Get_WhenAggregateIdentifierIsProvided_ShouldReturnSnapshot()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(snapshotDataModel);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        SnapshotDataModel result = snapshotStore.Get(aggregateIdentifier);

        // Assert
        result.Should().Be(snapshotDataModel);
    }

    [Test]
    public async Task GetAsync_WhenAggregateIdentifierIsProvided_ShouldReturnSnapshot()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(snapshotDataModel);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        SnapshotDataModel result = await snapshotStore.GetAsync(aggregateIdentifier, _cancellationToken);

        // Assert
        result.Should().Be(snapshotDataModel);
    }

    [Test]
    public void Get_WhenAggregateIdentifierIsProvidedAndSnapshotDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns((SnapshotDataModel)null);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        SnapshotDataModel result = snapshotStore.Get(aggregateIdentifier);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetAsync_WhenAggregateIdentifierIsProvidedAndSnapshotDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns((SnapshotDataModel)null);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        SnapshotDataModel result = await snapshotStore.GetAsync(aggregateIdentifier, _cancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void Save_WhenSnapshotIsProvided_ShouldSaveSnapshot()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        snapshotStore.Save(snapshotDataModel);

        // Assert
        _snapshotCacheMock.Verify(x => x.Add(aggregateIdentifier, snapshotDataModel), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenSnapshotIsProvided_ShouldSaveSnapshot()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        await snapshotStore.SaveAsync(snapshotDataModel, _cancellationToken);

        // Assert
        _snapshotCacheMock.Verify(x => x.Add(aggregateIdentifier, snapshotDataModel), Times.Once);
    }

    [Test]
    public void Save_WhenSnapshotIsProvidedAndSnapshotAlreadyExists_ShouldUpdateSnapshot()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(snapshotDataModel);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        snapshotStore.Save(snapshotDataModel);

        // Assert
        _snapshotCacheMock.Verify(x => x.Add(aggregateIdentifier, snapshotDataModel), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenSnapshotIsProvidedAndSnapshotAlreadyExists_ShouldUpdateSnapshot()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(snapshotDataModel);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        await snapshotStore.SaveAsync(snapshotDataModel, _cancellationToken);

        // Assert
        _snapshotCacheMock.Verify(x => x.Add(aggregateIdentifier, snapshotDataModel), Times.Once);
    }

    [Test]
    public void Save_WhenSnapshotIsProvidedAndSnapshotDoesNotExist_ShouldSaveSnapshot()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns((SnapshotDataModel)null);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        snapshotStore.Save(snapshotDataModel);

        // Assert
        _snapshotCacheMock.Verify(x => x.Add(aggregateIdentifier, snapshotDataModel), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenSnapshotIsProvidedAndSnapshotDoesNotExist_ShouldSaveSnapshot()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns((SnapshotDataModel)null);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        await snapshotStore.SaveAsync(snapshotDataModel, _cancellationToken);

        // Assert
        _snapshotCacheMock.Verify(x => x.Add(aggregateIdentifier, snapshotDataModel), Times.Once);
    }

    [Test]
    public void Box_WhenSnapshotIsProvided_ShouldBoxSnapshot()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(snapshotDataModel);
        _snapshotOfflineStorageMock.Setup(x => x.Save(snapshotDataModel, _encoding));
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        snapshotStore.Box(aggregateIdentifier, _encoding);

        // Assert
        _snapshotCacheMock.Verify(x => x.Get(aggregateIdentifier), Times.Once);
        _snapshotOfflineStorageMock.Verify(x => x.Save(snapshotDataModel, _encoding), Times.Once);
    }

    [Test]
    public async Task BoxAsync_WhenSnapshotIsProvided_ShouldBoxSnapshot()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(snapshotDataModel);
        _snapshotOfflineStorageMock.Setup(x => x.SaveAsync(snapshotDataModel, _encoding, _cancellationToken));
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        await snapshotStore.BoxAsync(aggregateIdentifier, _encoding, _cancellationToken);

        // Assert
        _snapshotCacheMock.Verify(x => x.Get(aggregateIdentifier), Times.Once);
        _snapshotOfflineStorageMock.Verify(x => x.SaveAsync(snapshotDataModel, _encoding, _cancellationToken),
            Times.Once);
    }

    [Test]
    public void Box_WhenSnapshotIsProvidedAndSnapshotDoesNotExist_ShouldThrowException()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns((SnapshotDataModel)null);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        Action act = () => snapshotStore.Box(aggregateIdentifier, _encoding);

        // Assert
        act.Should().ThrowExactly<SnapshotBoxingFailedException>().WithMessage(
            $"Unable to box snapshot for aggregate with id: ({aggregateIdentifier}), because snapshot hasn't been found. See inner exception for more details");
    }

    [Test]
    public async Task BoxAsync_WhenSnapshotIsProvidedAndSnapshotDoesNotExist_ShouldThrowException()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns((SnapshotDataModel)null);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        Func<Task> act = async () => await snapshotStore.BoxAsync(aggregateIdentifier, _encoding, _cancellationToken);

        // Assert
        await act.Should().ThrowExactlyAsync<SnapshotBoxingFailedException>().WithMessage(
            $"Unable to box snapshot for aggregate with id: ({aggregateIdentifier}), because snapshot hasn't been found. See inner exception for more details");
    }

    [Test]
    public void Unbox_WhenSnapshotIsProvided_ShouldUnboxSnapshot()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        _snapshotOfflineStorageMock.Setup(x => x.Restore(aggregateIdentifier, _encoding)).Returns(snapshotDataModel);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        SnapshotDataModel result = snapshotStore.Unbox(aggregateIdentifier, _encoding);

        // Assert
        result.Should().BeEquivalentTo(snapshotDataModel);
        _snapshotOfflineStorageMock.Verify(x => x.Restore(aggregateIdentifier, _encoding), Times.Once);
    }

    [Test]
    public async Task UnboxAsync_WhenSnapshotIsProvided_ShouldUnboxSnapshot()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        _snapshotOfflineStorageMock.Setup(x => x.RestoreAsync(aggregateIdentifier, _encoding, _cancellationToken))
            .ReturnsAsync(snapshotDataModel);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        SnapshotDataModel result = await snapshotStore.UnboxAsync(aggregateIdentifier, _encoding, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(snapshotDataModel);
        _snapshotOfflineStorageMock.Verify(x => x.RestoreAsync(aggregateIdentifier, _encoding, _cancellationToken),
            Times.Once);
    }

    [Test]
    public void Unbox_WhenSnapshotIsProvidedAndSnapshotDoesNotExist_ShouldThrowException()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        _snapshotOfflineStorageMock.Setup(x => x.Restore(aggregateIdentifier, _encoding))
            .Throws(SnapshotUnboxingFailedException.SnapshotNotFound(aggregateIdentifier));
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        Action act = () => snapshotStore.Unbox(aggregateIdentifier, _encoding);

        // Assert
        act.Should().ThrowExactly<SnapshotUnboxingFailedException>().WithMessage(
            $"Unable to unbox snapshot for aggregate with id: ({aggregateIdentifier}), because snapshot hasn't been found. See inner exception for more details");
    }

    [Test]
    public async Task UnboxAsync_WhenSnapshotIsProvidedAndSnapshotDoesNotExist_ShouldThrowException()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        _snapshotOfflineStorageMock.Setup(x => x.RestoreAsync(aggregateIdentifier, _encoding, _cancellationToken))
            .Throws(SnapshotUnboxingFailedException.SnapshotNotFound(aggregateIdentifier));
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        Func<Task> act = async () => await snapshotStore.UnboxAsync(aggregateIdentifier, _encoding, _cancellationToken);

        // Assert
        await act.Should().ThrowExactlyAsync<SnapshotUnboxingFailedException>().WithMessage(
            $"Unable to unbox snapshot for aggregate with id: ({aggregateIdentifier}), because snapshot hasn't been found. See inner exception for more details");
    }

    private SnapshotDataModel GetSnapshotDataModel(PostIdentifier aggregateIdentifier)
    {
        string serializedAggregateState = _faker.Random.String(_faker.Random.Int(1, 250));
        
        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = aggregateIdentifier,
            AggregateVersion = 1,
            AggregateState = serializedAggregateState
        };

        return snapshotDataModel;
    }
}