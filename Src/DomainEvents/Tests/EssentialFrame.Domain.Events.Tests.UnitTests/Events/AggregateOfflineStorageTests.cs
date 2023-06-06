using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Security;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Exceptions;
using EssentialFrame.Domain.Events.Persistence.DomainEvents;
using EssentialFrame.Domain.Events.Persistence.DomainEvents.Interfaces;
using EssentialFrame.Domain.Factories;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects;
using EssentialFrame.ExampleApp.Identity;
using EssentialFrame.Extensions;
using EssentialFrame.Files;
using EssentialFrame.Identity;
using EssentialFrame.Serialization.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.Events.Tests.UnitTests.Events;

[TestFixture]
public class AggregateOfflineStorageTests
{
    [SetUp]
    public void SetUp()
    {
        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new IdentityContext());
        _logger = NullLoggerFactory.Instance.CreateLogger<AggregateOfflineStorage>();
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;

        _aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion,
            _identityServiceMock.Object);

        _domainEvents = new List<IDomainEvent>
        {
            new ChangeTitleDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                Title.Create(_faker.Lorem.Sentence(), true)),
            new ChangeDescriptionDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                _faker.Lorem.Sentences())
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
    }

    private readonly Faker _faker = new();
    private readonly Mock<IFileStorage> _fileStorageMock = new();
    private readonly Mock<ISerializer> _serializerMock = new();
    private readonly Mock<IFileSystem> _fileSystemMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();

    private ILogger<AggregateOfflineStorage> _logger;
    private AggregateRoot _aggregate;
    private IReadOnlyCollection<IDomainEvent> _domainEvents;

    private static object[] _possibleExceptions =
    {
        new object[] { typeof(OutOfMemoryException) }, new object[] { typeof(ArgumentException) },
        new object[] { typeof(ArgumentNullException) }, new object[] { typeof(PathTooLongException) },
        new object[] { typeof(DirectoryNotFoundException) }, new object[] { typeof(IOException) },
        new object[] { typeof(UnauthorizedAccessException) }, new object[] { typeof(NotSupportedException) },
        new object[] { typeof(PathTooLongException) }, new object[] { typeof(UnauthorizedAccessException) },
        new object[] { typeof(SecurityException) }
    };

    [Test]
    public void Save_UsingDefaultDirectory_ShouldSaveFile()
    {
        // Arrange
        IAggregateOfflineStorage aggregateOfflineStorage = new AggregateOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger);

        Mock<IFileInfo> fileInfoMock = new();
        string directoryPath = _faker.System.DirectoryPath();

        fileInfoMock.Setup(x => x.Length).Returns(_faker.Random.Long(1, 5000));

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _aggregate.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileStorageMock.Setup(fs => fs.Create(directoryPath, It.IsAny<string>(), It.IsAny<string>(), null))
            .Returns(fileInfoMock.Object);

        _serializerMock.Setup(s => s.Serialize(_domainEvents)).Returns(_faker.Lorem.Sentences());

        // Act
        aggregateOfflineStorage.Save(_aggregate, _domainEvents);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _fileStorageMock.Verify(fs => fs.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null),
            Times.Exactly(3));

        _serializerMock.Verify(s => s.Serialize(_domainEvents), Times.Once);
    }

    [Test]
    public async Task SaveAsync_UsingDefaultDirectory_ShouldSaveFile()
    {
        // Arrange
        IAggregateOfflineStorage aggregateOfflineStorage = new AggregateOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger);

        Mock<IFileInfo> fileInfoMock = new();
        string directoryPath = _faker.System.DirectoryPath();

        fileInfoMock.Setup(x => x.Length).Returns(_faker.Random.Long(1, 5000));

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _aggregate.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileStorageMock
            .Setup(fs => fs.CreateAsync(directoryPath, It.IsAny<string>(), It.IsAny<string>(), null, default))
            .ReturnsAsync(fileInfoMock.Object);

        _serializerMock.Setup(s => s.Serialize(_domainEvents)).Returns(_faker.Lorem.Sentences());

        // Act
        await aggregateOfflineStorage.SaveAsync(_aggregate, _domainEvents);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _fileStorageMock.Verify(
            fs => fs.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, default),
            Times.Exactly(3));

        _serializerMock.Verify(s => s.Serialize(_domainEvents), Times.Once);
    }

    [Test]
    public void Save_UsingCustomDirectory_ShouldSaveFile()
    {
        // Arrange
        string offlineDirectory = _faker.System.DirectoryPath();

        IAggregateOfflineStorage aggregateOfflineStorage = new AggregateOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, offlineDirectory);

        Mock<IFileInfo> fileInfoMock = new();
        string directoryPath = _faker.System.DirectoryPath();

        fileInfoMock.Setup(x => x.Length).Returns(_faker.Random.Long(1, 5000));

        _fileSystemMock.Setup(x => x.Path.Combine(offlineDirectory, _aggregate.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileStorageMock.Setup(fs => fs.Create(directoryPath, It.IsAny<string>(), It.IsAny<string>(), null))
            .Returns(fileInfoMock.Object);

        _serializerMock.Setup(s => s.Serialize(_domainEvents)).Returns(_faker.Lorem.Sentences());

        // Act
        aggregateOfflineStorage.Save(_aggregate, _domainEvents);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _fileStorageMock.Verify(fs => fs.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null),
            Times.Exactly(3));

        _serializerMock.Verify(s => s.Serialize(_domainEvents), Times.Once);
    }

    [Test]
    public async Task SaveAsync_UsingCustomDirectory_ShouldSaveFile()
    {
        // Arrange
        string offlineDirectory = _faker.System.DirectoryPath();

        IAggregateOfflineStorage aggregateOfflineStorage = new AggregateOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, offlineDirectory);

        Mock<IFileInfo> fileInfoMock = new();
        string directoryPath = _faker.System.DirectoryPath();

        fileInfoMock.Setup(x => x.Length).Returns(_faker.Random.Long(1, 5000));

        _fileSystemMock.Setup(x => x.Path.Combine(offlineDirectory, _aggregate.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileStorageMock
            .Setup(fs => fs.CreateAsync(directoryPath, It.IsAny<string>(), It.IsAny<string>(), null, default))
            .ReturnsAsync(fileInfoMock.Object);

        _serializerMock.Setup(s => s.Serialize(_domainEvents)).Returns(_faker.Lorem.Sentences());

        // Act
        await aggregateOfflineStorage.SaveAsync(_aggregate, _domainEvents);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _fileStorageMock.Verify(
            fs => fs.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, default),
            Times.Exactly(3));

        _serializerMock.Verify(s => s.Serialize(_domainEvents), Times.Once);
    }

    [Test]
    [TestCaseSource(nameof(_possibleExceptions))]
    public void Save_WhenExceptionOccurs_ShouldCatchAndThrowAggregateBoxingFailedException(Type exception)
    {
        // Arrange
        IAggregateOfflineStorage aggregateOfflineStorage = new AggregateOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _aggregate.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileStorageMock.Setup(fs => fs.Create(directoryPath, It.IsAny<string>(), It.IsAny<string>(), null))
            .Throws(Activator.CreateInstance(exception) as Exception);

        // Act
        Action action = () => aggregateOfflineStorage.Save(_aggregate, _domainEvents);

        // Assert
        action.Should().ThrowExactly<AggregateBoxingFailedException>().WithMessage(
                $"Unable to box aggregate ({_aggregate.GetTypeFullName()}) with id: ({_aggregate.AggregateIdentifier}). See inner exception for more details.")
            .WithInnerExceptionExactly(exception);
    }

    [Test]
    [TestCaseSource(nameof(_possibleExceptions))]
    public async Task SaveAsync_WhenExceptionOccurs_ShouldCatchAndThrowAggregateBoxingFailedException(Type exception)
    {
        // Arrange
        IAggregateOfflineStorage aggregateOfflineStorage = new AggregateOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _aggregate.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileStorageMock
            .Setup(fs => fs.CreateAsync(directoryPath, It.IsAny<string>(), It.IsAny<string>(), null, default))
            .ThrowsAsync(Activator.CreateInstance(exception) as Exception);

        // Act
        Func<Task> action = async () => await aggregateOfflineStorage.SaveAsync(_aggregate, _domainEvents);

        // Assert
        await action.Should().ThrowExactlyAsync<AggregateBoxingFailedException>().WithMessage(
                $"Unable to box aggregate ({_aggregate.GetTypeFullName()}) with id: ({_aggregate.AggregateIdentifier}). See inner exception for more details.")
            .WithInnerExceptionExactly(exception);
    }
}