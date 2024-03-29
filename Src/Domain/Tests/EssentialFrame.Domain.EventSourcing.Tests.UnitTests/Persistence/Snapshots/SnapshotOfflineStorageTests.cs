﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Domain.EventSourcing.Exceptions;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Models;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Services;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Services.Interfaces;
using EssentialFrame.ExampleApp.Application.Identity;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.Files.Interfaces;
using EssentialFrame.Identity.Interfaces;
using EssentialFrame.Serialization.Interfaces;
using EssentialFrame.Tests.Utils;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.EventSourcing.Tests.UnitTests.Persistence.Snapshots;

[TestFixture]
public class SnapshotOfflineStorageTests
{
    private const string OfflineStorageDirectory = default;
    
    private readonly Faker _faker = new();
    private readonly Mock<IFileStorage> _fileStorageMock = new();
    private readonly Mock<ISerializer> _serializerMock = new();
    private readonly Mock<IFileSystem> _fileSystemMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly CancellationToken _cancellationToken = default;
    
    private readonly IList<Encoding> _encodings = new List<Encoding>
    {
        Encoding.Default,
        Encoding.Unicode,
        Encoding.UTF8,
        Encoding.UTF32,
        Encoding.ASCII
    };

    private ILogger<SnapshotOfflineStorage> _logger;
    private SnapshotDataModel _snapshotDataModel;
    private Encoding _encoding;

    private static readonly object[] PossibleExceptions =
    {
        new object[] { typeof(OutOfMemoryException) }, new object[] { typeof(ArgumentException) },
        new object[] { typeof(ArgumentNullException) }, new object[] { typeof(PathTooLongException) },
        new object[] { typeof(DirectoryNotFoundException) }, new object[] { typeof(IOException) },
        new object[] { typeof(UnauthorizedAccessException) }, new object[] { typeof(NotSupportedException) },
        new object[] { typeof(PathTooLongException) }, new object[] { typeof(UnauthorizedAccessException) },
        new object[] { typeof(SecurityException) }
    };
    
    [SetUp]
    public void SetUp()
    {
        _encoding = _faker.Random.ListItem(_encodings);
        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new AppIdentityContext());
        _logger = NullLoggerFactory.Instance.CreateLogger<SnapshotOfflineStorage>();

        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;
        string aggregateState = _faker.Lorem.Sentences();

        _snapshotDataModel = new SnapshotDataModel
        {
            AggregateIdentifier = aggregateIdentifier,
            AggregateVersion = aggregateVersion,
            AggregateState = aggregateState
        };
    }

    [TearDown]
    public void TearDown()
    {
        _fileStorageMock.Reset();
        _serializerMock.Reset();
        _fileSystemMock.Reset();
        _identityServiceMock.Reset();
        _logger = null;
        _encoding = null;
    }

    [Test]
    public void Save_UsingDefaultDirectory_ShouldSaveFile()
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, OfflineStorageDirectory);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _snapshotDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        // Act
        snapShotOfflineStorage.Save(_snapshotDataModel, _encoding);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), _snapshotDataModel.AggregateIdentifier),
            Times.Once);

        _fileStorageMock.Verify(
            x => x.Create(directoryPath, It.IsAny<string>(), _snapshotDataModel.AggregateState.ToString(), _encoding),
            Times.Once);
    }

    [Test]
    public async Task SaveAsync_UsingDefaultDirectory_ShouldSaveFile()
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, OfflineStorageDirectory);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _snapshotDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        // Act
        await snapShotOfflineStorage.SaveAsync(_snapshotDataModel, _encoding, _cancellationToken);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), _snapshotDataModel.AggregateIdentifier),
            Times.Once);

        _fileStorageMock.Verify(x => x.CreateAsync(directoryPath, It.IsAny<string>(),
            _snapshotDataModel.AggregateState.ToString(), _encoding, _cancellationToken), Times.Once);
    }

    [Test]
    public void Save_UsingCustomDirectory_ShouldSaveFile()
    {
        // Arrange
        string offlineDirectory = _faker.System.DirectoryPath();

        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, offlineDirectory);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(offlineDirectory, _snapshotDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        // Act
        snapShotOfflineStorage.Save(_snapshotDataModel, _encoding);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), _snapshotDataModel.AggregateIdentifier),
            Times.Once);

        _fileStorageMock.Verify(
            x => x.Create(directoryPath, It.IsAny<string>(), _snapshotDataModel.AggregateState.ToString(), _encoding),
            Times.Once);
    }

    [Test]
    public async Task SaveAsync_UsingCustomDirectory_ShouldSaveFile()
    {
        // Arrange
        string offlineDirectory = _faker.System.DirectoryPath();

        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, offlineDirectory);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(offlineDirectory, _snapshotDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        // Act
        await snapShotOfflineStorage.SaveAsync(_snapshotDataModel, _encoding, _cancellationToken);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), _snapshotDataModel.AggregateIdentifier),
            Times.Once);

        _fileStorageMock.Verify(x => x.CreateAsync(directoryPath, It.IsAny<string>(),
            _snapshotDataModel.AggregateState.ToString(), _encoding, _cancellationToken), Times.Once);
    }

    [Test]
    public void Save_WhenStateIsAnObject_ShouldSaveFile()
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, OfflineStorageDirectory);

        string directoryPath = _faker.System.DirectoryPath();
        string[] state = _faker.Random.WordsArray(10);
        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = _snapshotDataModel.AggregateIdentifier,
            AggregateVersion = _snapshotDataModel.AggregateVersion,
            AggregateState = state
        };
        string serializedState = _faker.Lorem.Sentences();

        _serializerMock.Setup(x => x.Serialize(snapshotDataModel.AggregateState)).Returns(serializedState);

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), snapshotDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        // Act
        snapShotOfflineStorage.Save(snapshotDataModel, _encoding);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), snapshotDataModel.AggregateIdentifier),
            Times.Once);

        _serializerMock.Verify(x => x.Serialize(snapshotDataModel.AggregateState), Times.Once);

        _fileStorageMock.Verify(x => x.Create(directoryPath, It.IsAny<string>(), serializedState, _encoding),
            Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenStateIsAnObject_ShouldSaveFile()
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, OfflineStorageDirectory);

        string directoryPath = _faker.System.DirectoryPath();
        string[] state = _faker.Random.WordsArray(10);
        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = _snapshotDataModel.AggregateIdentifier,
            AggregateVersion = _snapshotDataModel.AggregateVersion,
            AggregateState = state
        };
        string serializedState = _faker.Lorem.Sentences();

        _serializerMock.Setup(x => x.Serialize(snapshotDataModel.AggregateState)).Returns(serializedState);

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), snapshotDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        // Act
        await snapShotOfflineStorage.SaveAsync(snapshotDataModel, _encoding, _cancellationToken);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), snapshotDataModel.AggregateIdentifier),
            Times.Once);

        _serializerMock.Verify(x => x.Serialize(snapshotDataModel.AggregateState), Times.Once);

        _fileStorageMock.Verify(
            x => x.CreateAsync(directoryPath, It.IsAny<string>(), serializedState, _encoding, _cancellationToken),
            Times.Once);
    }

    [Test]
    [TestCaseSource(nameof(PossibleExceptions))]
    public void Save_WhenExceptionOccurs_ShouldCatchAndThrowAggregateBoxingFailedException(Type exception)
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, OfflineStorageDirectory);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _snapshotDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        _fileStorageMock.Setup(x =>
                x.Create(directoryPath, It.IsAny<string>(), _snapshotDataModel.AggregateState.ToString(), _encoding))
            .Throws((Activator.CreateInstance(exception) as Exception)!);

        // Act
        Action action = () => snapShotOfflineStorage.Save(_snapshotDataModel, _encoding);

        // Assert
        action.Should().ThrowExactly<SnapshotBoxingFailedException>().WithMessage(
                $"Unexpected error while boxing snapshot for aggregate with id: ({_snapshotDataModel.AggregateIdentifier}). See inner exception for more details")
            .WithInnerExceptionExactly(exception);
    }

    [Test]
    [TestCaseSource(nameof(PossibleExceptions))]
    public async Task SaveAsync_WhenExceptionOccurs_ShouldCatchAndThrowAggregateBoxingFailedException(Type exception)
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, OfflineStorageDirectory);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _snapshotDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        _fileStorageMock.Setup(x => x.CreateAsync(directoryPath, It.IsAny<string>(),
                _snapshotDataModel.AggregateState.ToString(), _encoding, _cancellationToken))
            .Throws((Activator.CreateInstance(exception) as Exception)!);

        // Act
        Func<Task> action = async () =>
            await snapShotOfflineStorage.SaveAsync(_snapshotDataModel, _encoding, _cancellationToken);

        // Assert
        await action.Should().ThrowExactlyAsync<SnapshotBoxingFailedException>().WithMessage(
                $"Unexpected error while boxing snapshot for aggregate with id: ({_snapshotDataModel.AggregateIdentifier}). See inner exception for more details")
            .WithInnerExceptionExactly(exception);
    }

    [Test]
    public void Restore_UsingDefaultDirectory_ShouldRestoreAggregate()
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, OfflineStorageDirectory);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _snapshotDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        string serializedState = _faker.Lorem.Sentences();

        _fileStorageMock.Setup(x => x.Read(directoryPath, It.IsAny<string>(), _encoding)).Returns(serializedState);

        // Act
        SnapshotDataModel snapshot = snapShotOfflineStorage.Get(_snapshotDataModel.AggregateIdentifier, _encoding);

        // Assert
        snapshot.AggregateIdentifier.Should().Be(_snapshotDataModel.AggregateIdentifier);
        snapshot.AggregateVersion.Should().Be(Defaults.DefaultAggregateVersion);
        snapshot.AggregateState.Should().Be(serializedState);
    }

    [Test]
    public async Task RestoreAsync_UsingDefaultDirectory_ShouldRestoreAggregate()
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, OfflineStorageDirectory);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _snapshotDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        string serializedState = _faker.Lorem.Sentences();

        _fileStorageMock.Setup(x => x.ReadAsync(directoryPath, It.IsAny<string>(), _encoding, _cancellationToken))
            .ReturnsAsync(serializedState);

        // Act
        SnapshotDataModel snapshot = await snapShotOfflineStorage.GetAsync(_snapshotDataModel.AggregateIdentifier,
            _encoding, _cancellationToken);

        // Assert
        snapshot.AggregateIdentifier.Should().Be(_snapshotDataModel.AggregateIdentifier);
        snapshot.AggregateVersion.Should().Be(Defaults.DefaultAggregateVersion);
        snapshot.AggregateState.Should().Be(serializedState);
    }

    [Test]
    public void Restore_UsingCustomDirectory_ShouldRestoreAggregate()
    {
        // Arrange
        string offlineDirectory = _faker.System.DirectoryPath();

        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, offlineDirectory);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(offlineDirectory, _snapshotDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        string serializedState = _faker.Lorem.Sentences();

        _fileStorageMock.Setup(x => x.Read(directoryPath, It.IsAny<string>(), _encoding)).Returns(serializedState);

        // Act
        SnapshotDataModel snapshot = snapShotOfflineStorage.Get(_snapshotDataModel.AggregateIdentifier, _encoding);

        // Assert
        snapshot.AggregateIdentifier.Should().Be(_snapshotDataModel.AggregateIdentifier);
        snapshot.AggregateVersion.Should().Be(Defaults.DefaultAggregateVersion);
        snapshot.AggregateState.Should().Be(serializedState);
    }

    [Test]
    public void RestoreAsync_UsingCustomDirectory_ShouldRestoreAggregate()
    {
        // Arrange
        string offlineDirectory = _faker.System.DirectoryPath();

        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, offlineDirectory);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(offlineDirectory, _snapshotDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        string serializedState = _faker.Lorem.Sentences();

        _fileStorageMock.Setup(x => x.ReadAsync(directoryPath, It.IsAny<string>(), _encoding, _cancellationToken))
            .ReturnsAsync(serializedState);

        // Act
        SnapshotDataModel snapshot = snapShotOfflineStorage
            .GetAsync(_snapshotDataModel.AggregateIdentifier, _encoding, _cancellationToken)
            .GetAwaiter().GetResult();

        // Assert
        snapshot.AggregateIdentifier.Should().Be(_snapshotDataModel.AggregateIdentifier);
        snapshot.AggregateVersion.Should().Be(Defaults.DefaultAggregateVersion);
        snapshot.AggregateState.Should().Be(serializedState);
    }

    [Test]
    public void Restore_WhenAggregateNotFound_ShouldThrowFileNotFoundException()
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, OfflineStorageDirectory);

        string directoryPath = _faker.System.DirectoryPath();
        string filePath = _faker.System.FilePath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _snapshotDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        _fileSystemMock.Setup(x => x.Path.Combine(directoryPath, filePath)).Returns(directoryPath + filePath);

        FileNotFoundException fileNotFoundException =
            new($"File not found: {_fileSystemMock.Object.Path.Combine(directoryPath, filePath)}");

        _fileStorageMock.Setup(x => x.Read(directoryPath, It.IsAny<string>(), _encoding)).Throws(fileNotFoundException);

        // Act
        Action action = () => snapShotOfflineStorage.Get(_snapshotDataModel.AggregateIdentifier, _encoding);

        // Assert
        action.Should().ThrowExactly<SnapshotUnboxingFailedException>().WithMessage(
                $"Unexpected error while unboxing snapshot for aggregate with id: ({_snapshotDataModel.AggregateIdentifier}). See inner exception for more details")
            .WithInnerExceptionExactly<FileNotFoundException>().WithMessage(fileNotFoundException.Message);
    }

    [Test]
    public async Task RestoreAsync_WhenAggregateNotFound_ShouldThrowFileNotFoundException()
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, OfflineStorageDirectory);

        string directoryPath = _faker.System.DirectoryPath();
        string filePath = _faker.System.FilePath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _snapshotDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        _fileSystemMock.Setup(x => x.Path.Combine(directoryPath, filePath)).Returns(directoryPath + filePath);

        FileNotFoundException fileNotFoundException =
            new($"File not found: {_fileSystemMock.Object.Path.Combine(directoryPath, filePath)}");

        _fileStorageMock.Setup(x => x.ReadAsync(directoryPath, It.IsAny<string>(), _encoding, _cancellationToken))
            .Throws(fileNotFoundException);

        // Act
        Func<Task> action = async () => await snapShotOfflineStorage.GetAsync(_snapshotDataModel.AggregateIdentifier,
            _encoding,
                _cancellationToken);

        // Assert
        await action.Should().ThrowExactlyAsync<SnapshotUnboxingFailedException>().WithMessage(
                $"Unexpected error while unboxing snapshot for aggregate with id: ({_snapshotDataModel.AggregateIdentifier}). See inner exception for more details")
            .WithInnerExceptionExactly(fileNotFoundException.GetType()).WithMessage(fileNotFoundException.Message);
    }

    [Test]
    [TestCaseSource(nameof(PossibleExceptions))]
    public void Restore_WhenExceptionOccurs_ShouldThrowAggregateUnBoxingFailedException(Type exception)
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, OfflineStorageDirectory);

        string directoryPath = _faker.System.DirectoryPath();
        string filePath = _faker.System.FilePath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _snapshotDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        _fileSystemMock.Setup(x => x.Path.Combine(directoryPath, filePath)).Returns(directoryPath + filePath);
        _fileStorageMock.Setup(x => x.Read(directoryPath, It.IsAny<string>(), _encoding))
            .Throws((Activator.CreateInstance(exception) as Exception)!);

        // Act
        Action action = () => snapShotOfflineStorage.Get(_snapshotDataModel.AggregateIdentifier, _encoding);

        // Assert
        action.Should().ThrowExactly<SnapshotUnboxingFailedException>().WithMessage(
                $"Unexpected error while unboxing snapshot for aggregate with id: ({_snapshotDataModel.AggregateIdentifier}). See inner exception for more details")
            .WithInnerExceptionExactly(exception);
    }

    [Test]
    [TestCaseSource(nameof(PossibleExceptions))]
    public async Task RestoreAsync_WhenExceptionOccurs_ShouldThrowAggregateUnBoxingFailedException(Type exception)
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, OfflineStorageDirectory);

        string directoryPath = _faker.System.DirectoryPath();
        string filePath = _faker.System.FilePath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _snapshotDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        _fileSystemMock.Setup(x => x.Path.Combine(directoryPath, filePath)).Returns(directoryPath + filePath);
        _fileStorageMock.Setup(x => x.ReadAsync(directoryPath, It.IsAny<string>(), _encoding, _cancellationToken))
            .Throws((Activator.CreateInstance(exception) as Exception)!);

        // Act
        Func<Task> action = async () => await snapShotOfflineStorage.GetAsync(_snapshotDataModel.AggregateIdentifier,
            _encoding,
                _cancellationToken);

        // Assert
        await action.Should().ThrowExactlyAsync<SnapshotUnboxingFailedException>().WithMessage(
                $"Unexpected error while unboxing snapshot for aggregate with id: ({_snapshotDataModel.AggregateIdentifier}). See inner exception for more details")
            .WithInnerExceptionExactly(exception);
    }
}