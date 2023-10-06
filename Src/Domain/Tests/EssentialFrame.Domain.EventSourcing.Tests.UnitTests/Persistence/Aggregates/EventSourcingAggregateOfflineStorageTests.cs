using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.EventSourcing.Core.Factories;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Persistence.Mappers.Interfaces;
using EssentialFrame.Domain.Persistence.Models;
using EssentialFrame.ExampleApp.Application.Identity;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents.Events;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;
using EssentialFrame.Extensions;
using EssentialFrame.Files.Interfaces;
using EssentialFrame.Identity.Interfaces;
using EssentialFrame.Serialization.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.EventSourcing.Tests.UnitTests.Persistence.Aggregates;

[TestFixture]
public class EventSourcingAggregateOfflineStorageTests
{
    private readonly Faker _faker = new();
    private readonly Mock<IFileStorage> _fileStorageMock = new();
    private readonly Mock<ISerializer> _serializerMock = new();
    private readonly Mock<IFileSystem> _fileSystemMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly Mock<IDomainEventMapper<PostIdentifier, Guid>> _domainEventMapperMock = new();

    private readonly IList<Encoding> _encodings = new List<Encoding>
    {
        Encoding.Default,
        Encoding.Unicode,
        Encoding.UTF8,
        Encoding.UTF32,
        Encoding.ASCII
    };
    
    private EventSourcingAggregateDataModel _eventSourcingAggregateDataModel;
    private ILogger<EventSourcingAggregateOfflineStorage<PostIdentifier, Guid>> _logger;
    private IReadOnlyCollection<DomainEventDataModel> _domainEventDataModels;
    private IReadOnlyCollection<IDomainEvent<PostIdentifier, Guid>> _domainEvents;
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
        _logger = NullLoggerFactory.Instance.CreateLogger<EventSourcingAggregateOfflineStorage<PostIdentifier, Guid>>();

        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        _eventSourcingAggregateDataModel = new EventSourcingAggregateDataModel
        {
            AggregateIdentifier = aggregateIdentifier,
            AggregateVersion = aggregateVersion,
            DeletedDate = aggregate.DeletedDate,
            IsDeleted = aggregate.IsDeleted,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent().Tenant.Identifier
        };

        _domainEvents = new List<IDomainEvent<PostIdentifier, Guid>>
        {
            new TitleChangedDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                Title.Default(_faker.Lorem.Sentence())),
            new DescriptionChangedDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                Description.Create(_faker.Lorem.Sentences()))
        };

        _domainEventDataModels = _domainEvents.Select(domainEvent => new DomainEventDataModel
        {
            AggregateIdentifier = domainEvent.AggregateIdentifier,
            AggregateVersion = domainEvent.AggregateVersion,
            EventIdentifier = domainEvent.EventIdentifier,
            EventType = domainEvent.GetTypeFullName(),
            EventClass = domainEvent.GetClassName(),
            DomainEvent = _serializerMock.Object.Serialize(domainEvent),
            CreatedAt = domainEvent.EventTime
        }).ToList();
    }

    [TearDown]
    public void TearDown()
    {
        _fileStorageMock.Reset();
        _serializerMock.Reset();
        _fileSystemMock.Reset();
        _identityServiceMock.Reset();
        _domainEventMapperMock.Reset();
        _logger = null;
        _encoding = null;
    }

    [Test]
    public void Save_UsingDefaultDirectory_ShouldSaveFile()
    {
        // Arrange
        IEventSourcingAggregateOfflineStorage eventSourcingAggregateOfflineStorage =
            new EventSourcingAggregateOfflineStorage<PostIdentifier, Guid>(_fileStorageMock.Object,
                _fileSystemMock.Object,
                _logger, _domainEventMapperMock.Object, _serializerMock.Object);

        Mock<IFileInfo> fileInfoMock = new();
        string directoryPath = _faker.System.DirectoryPath();

        fileInfoMock.Setup(x => x.Length).Returns(_faker.Random.Long(1, 5000));

        _fileSystemMock.Setup(x =>
                x.Path.Combine(It.IsAny<string>(), _eventSourcingAggregateDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        _fileStorageMock.Setup(fs => fs.Create(directoryPath, It.IsAny<string>(), It.IsAny<string>(), _encoding))
            .Returns(fileInfoMock.Object);

        _domainEventMapperMock.Setup(s => s.Map(_domainEventDataModels)).Returns(_domainEvents);
        _serializerMock.Setup(s => s.Serialize(_domainEvents)).Returns(_faker.Lorem.Sentences());

        // Act
        eventSourcingAggregateOfflineStorage.Save(_eventSourcingAggregateDataModel, _domainEventDataModels, _encoding);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _fileStorageMock.Verify(fs => fs.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), _encoding),
            Times.Exactly(3));

        _domainEventMapperMock.Verify(s => s.Map(_domainEventDataModels), Times.Once);
        _serializerMock.Verify(s => s.Serialize(_domainEvents), Times.Once);
    }

    [Test]
    public async Task SaveAsync_UsingDefaultDirectory_ShouldSaveFile()
    {
        // Arrange
        IEventSourcingAggregateOfflineStorage eventSourcingAggregateOfflineStorage =
            new EventSourcingAggregateOfflineStorage<PostIdentifier, Guid>(_fileStorageMock.Object,
                _fileSystemMock.Object,
                _logger, _domainEventMapperMock.Object, _serializerMock.Object);

        Mock<IFileInfo> fileInfoMock = new();
        string directoryPath = _faker.System.DirectoryPath();

        fileInfoMock.Setup(x => x.Length).Returns(_faker.Random.Long(1, 5000));

        _fileSystemMock.Setup(x =>
                x.Path.Combine(It.IsAny<string>(), _eventSourcingAggregateDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        _fileStorageMock.Setup(fs =>
                fs.CreateAsync(directoryPath, It.IsAny<string>(), It.IsAny<string>(), _encoding, default))
            .ReturnsAsync(fileInfoMock.Object);

        _domainEventMapperMock.Setup(s => s.Map(_domainEventDataModels)).Returns(_domainEvents);
        _serializerMock.Setup(s => s.Serialize(_domainEvents)).Returns(_faker.Lorem.Sentences());

        // Act
        await eventSourcingAggregateOfflineStorage.SaveAsync(_eventSourcingAggregateDataModel, _domainEventDataModels,
            _encoding);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _fileStorageMock.Verify(
            fs => fs.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), _encoding, default),
            Times.Exactly(3));

        _domainEventMapperMock.Verify(s => s.Map(_domainEventDataModels), Times.Once);
        _serializerMock.Verify(s => s.Serialize(_domainEvents), Times.Once);
    }

    [Test]
    public void Save_UsingCustomDirectory_ShouldSaveFile()
    {
        // Arrange
        string offlineDirectory = _faker.System.DirectoryPath();

        // Arrange
        IEventSourcingAggregateOfflineStorage eventSourcingAggregateOfflineStorage =
            new EventSourcingAggregateOfflineStorage<PostIdentifier, Guid>(_fileStorageMock.Object,
                _fileSystemMock.Object,
                _logger, _domainEventMapperMock.Object, _serializerMock.Object, offlineDirectory);

        Mock<IFileInfo> fileInfoMock = new();
        string directoryPath = _faker.System.DirectoryPath();

        fileInfoMock.Setup(x => x.Length).Returns(_faker.Random.Long(1, 5000));

        _fileSystemMock
            .Setup(x => x.Path.Combine(offlineDirectory, _eventSourcingAggregateDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        _fileStorageMock.Setup(fs => fs.Create(directoryPath, It.IsAny<string>(), It.IsAny<string>(), _encoding))
            .Returns(fileInfoMock.Object);

        _domainEventMapperMock.Setup(s => s.Map(_domainEventDataModels)).Returns(_domainEvents);
        _serializerMock.Setup(s => s.Serialize(_domainEvents)).Returns(_faker.Lorem.Sentences());

        // Act
        eventSourcingAggregateOfflineStorage.Save(_eventSourcingAggregateDataModel, _domainEventDataModels, _encoding);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _fileStorageMock.Verify(fs => fs.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), _encoding),
            Times.Exactly(3));

        _domainEventMapperMock.Verify(s => s.Map(_domainEventDataModels), Times.Once);
        _serializerMock.Verify(s => s.Serialize(_domainEvents), Times.Once);
    }

    [Test]
    public async Task SaveAsync_UsingCustomDirectory_ShouldSaveFile()
    {
        // Arrange
        string offlineDirectory = _faker.System.DirectoryPath();

        // Arrange
        IEventSourcingAggregateOfflineStorage eventSourcingAggregateOfflineStorage =
            new EventSourcingAggregateOfflineStorage<PostIdentifier, Guid>(_fileStorageMock.Object,
                _fileSystemMock.Object,
                _logger, _domainEventMapperMock.Object, _serializerMock.Object, offlineDirectory);

        Mock<IFileInfo> fileInfoMock = new();
        string directoryPath = _faker.System.DirectoryPath();

        fileInfoMock.Setup(x => x.Length).Returns(_faker.Random.Long(1, 5000));

        _fileSystemMock
            .Setup(x => x.Path.Combine(offlineDirectory, _eventSourcingAggregateDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        _fileStorageMock.Setup(fs =>
                fs.CreateAsync(directoryPath, It.IsAny<string>(), It.IsAny<string>(), _encoding, default))
            .ReturnsAsync(fileInfoMock.Object);

        _domainEventMapperMock.Setup(s => s.Map(_domainEventDataModels)).Returns(_domainEvents);
        _serializerMock.Setup(s => s.Serialize(_domainEvents)).Returns(_faker.Lorem.Sentences());

        // Act
        await eventSourcingAggregateOfflineStorage.SaveAsync(_eventSourcingAggregateDataModel, _domainEventDataModels,
            _encoding);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _fileStorageMock.Verify(
            fs => fs.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), _encoding, default),
            Times.Exactly(3));

        _domainEventMapperMock.Verify(s => s.Map(_domainEventDataModels), Times.Once);
        _serializerMock.Verify(s => s.Serialize(_domainEvents), Times.Once);
    }

    [Test]
    [TestCaseSource(nameof(PossibleExceptions))]
    public void Save_WhenExceptionOccurs_ShouldCatchAndThrowAggregateBoxingFailedException(Type exception)
    {
        // Arrange
        IEventSourcingAggregateOfflineStorage eventSourcingAggregateOfflineStorage =
            new EventSourcingAggregateOfflineStorage<PostIdentifier, Guid>(_fileStorageMock.Object,
                _fileSystemMock.Object,
                _logger, _domainEventMapperMock.Object, _serializerMock.Object);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x =>
                x.Path.Combine(It.IsAny<string>(), _eventSourcingAggregateDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        _fileStorageMock.Setup(fs => fs.Create(directoryPath, It.IsAny<string>(), It.IsAny<string>(), _encoding))
            .Throws((Activator.CreateInstance(exception) as Exception)!);

        // Act
        Action action = () =>
            eventSourcingAggregateOfflineStorage.Save(_eventSourcingAggregateDataModel, _domainEventDataModels,
                _encoding);

        // Assert
        action.Should().ThrowExactly<AggregateBoxingFailedException>().WithMessage(
                $"Unable to box aggregate ({_eventSourcingAggregateDataModel.GetTypeFullName()}) with id: ({_eventSourcingAggregateDataModel.AggregateIdentifier}). See inner exception for more details")
            .WithInnerExceptionExactly(exception);
    }

    [Test]
    [TestCaseSource(nameof(PossibleExceptions))]
    public async Task SaveAsync_WhenExceptionOccurs_ShouldCatchAndThrowAggregateBoxingFailedException(Type exception)
    {
        // Arrange
        IEventSourcingAggregateOfflineStorage eventSourcingAggregateOfflineStorage =
            new EventSourcingAggregateOfflineStorage<PostIdentifier, Guid>(_fileStorageMock.Object,
                _fileSystemMock.Object,
                _logger, _domainEventMapperMock.Object, _serializerMock.Object);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x =>
                x.Path.Combine(It.IsAny<string>(), _eventSourcingAggregateDataModel.AggregateIdentifier))
            .Returns(directoryPath);

        _fileStorageMock.Setup(fs =>
                fs.CreateAsync(directoryPath, It.IsAny<string>(), It.IsAny<string>(), _encoding, default))
            .ThrowsAsync((Activator.CreateInstance(exception) as Exception)!);

        // Act
        Func<Task> action = async () =>
            await eventSourcingAggregateOfflineStorage.SaveAsync(_eventSourcingAggregateDataModel,
                _domainEventDataModels, _encoding);

        // Assert
        await action.Should().ThrowExactlyAsync<AggregateBoxingFailedException>().WithMessage(
                $"Unable to box aggregate ({_eventSourcingAggregateDataModel.GetTypeFullName()}) with id: ({_eventSourcingAggregateDataModel.AggregateIdentifier}). See inner exception for more details")
            .WithInnerExceptionExactly(exception);
    }
}