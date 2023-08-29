using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.Factories;
using EssentialFrame.ExampleApp.Application.Identity;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;
using EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.BytesContents;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;
using EssentialFrame.Extensions;
using EssentialFrame.Identity;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.Events.Tests.UnitTests.Persistence.Aggregates;

[TestFixture]
public class DefaultAggregateStoreTests
{
    [SetUp]
    public void SetUp()
    {
        _aggregateCacheMock = new Mock<ICache<Guid, AggregateDataModel>>();
        _eventsCacheMock = new Mock<ICache<Guid, DomainEventDataModel>>();
        _identityServiceMock = new Mock<IIdentityService>();
        _aggregateOfflineStorageMock = new Mock<IAggregateOfflineStorage>();
        _domainEventMapper = new Mock<IDomainEventMapper>();
        _aggregateMapperMock = new Mock<IAggregateMapper>();

        _identityServiceMock.Setup(x => x.GetCurrent()).Returns(new IdentityContext());
    }

    [TearDown]
    public void TearDown()
    {
        _aggregateCacheMock.Reset();
        _eventsCacheMock.Reset();
        _identityServiceMock.Reset();
        _aggregateOfflineStorageMock.Reset();
        _domainEventMapper.Reset();
        _aggregateMapperMock.Reset();
    }

    private readonly Faker _faker = new();
    private Mock<ICache<Guid, AggregateDataModel>> _aggregateCacheMock;
    private Mock<ICache<Guid, DomainEventDataModel>> _eventsCacheMock;
    private Mock<IIdentityService> _identityServiceMock;
    private Mock<IAggregateOfflineStorage> _aggregateOfflineStorageMock;
    private Mock<IDomainEventMapper> _domainEventMapper;
    private Mock<IAggregateMapper> _aggregateMapperMock;

    [Test]
    public void Exists_WhenAggregateIdentifierIsProvided_ShouldReturnTrue()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        _aggregateCacheMock.Setup(x => x.Exists(aggregateIdentifier)).Returns(true);
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = aggregateStore.Exists(aggregateIdentifier);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task ExistsAsync_WhenAggregateIdentifierIsProvided_ShouldReturnTrue()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        _aggregateCacheMock.Setup(x => x.Exists(aggregateIdentifier)).Returns(true);
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = await aggregateStore.ExistsAsync(aggregateIdentifier);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Exists_WhenAggregateIdentifierIsProvided_ShouldReturnFalse()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        _aggregateCacheMock.Setup(x => x.Exists(aggregateIdentifier)).Returns(false);
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = aggregateStore.Exists(aggregateIdentifier);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task ExistsAsync_WhenAggregateIdentifierIsProvided_ShouldReturnFalse()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        _aggregateCacheMock.Setup(x => x.Exists(aggregateIdentifier)).Returns(false);
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = await aggregateStore.ExistsAsync(aggregateIdentifier);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Exists_WhenAggregateIdentifierAndVersionAreProvided_ShouldReturnTrue()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        _aggregateCacheMock.Setup(x => x.Exists(It.IsAny<Func<Guid, AggregateDataModel, bool>>())).Returns(true);
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = aggregateStore.Exists(aggregateIdentifier, aggregateVersion);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task ExistsAsync_WhenAggregateIdentifierAndVersionAreProvided_ShouldReturnTrue()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        int aggregateVersion = _faker.Random.Int();
        _aggregateCacheMock.Setup(x => x.Exists(It.IsAny<Func<Guid, AggregateDataModel, bool>>())).Returns(true);
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = await aggregateStore.ExistsAsync(aggregateIdentifier, aggregateVersion);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Exists_WhenAggregateIdentifierAndVersionAreProvided_ShouldReturnFalse()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        _aggregateCacheMock.Setup(x => x.Exists(It.IsAny<Func<Guid, AggregateDataModel, bool>>())).Returns(false);
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = aggregateStore.Exists(aggregateIdentifier, aggregateVersion);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task ExistsAsync_WhenAggregateIdentifierAndVersionAreProvided_ShouldReturnFalse()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        int aggregateVersion = _faker.Random.Int();
        _aggregateCacheMock.Setup(x => x.Exists(It.IsAny<Func<Guid, AggregateDataModel, bool>>())).Returns(false);
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = await aggregateStore.ExistsAsync(aggregateIdentifier, aggregateVersion);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Get_WhenAggregateIdentifierIsProvided_ShouldReturnAggregate()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();

        AggregateDataModel aggregateDataModel = new()
        {
            AggregateIdentifier = _faker.Random.Guid(),
            AggregateVersion = _faker.Random.Int(),
            DeletedDate = null,
            IsDeleted = false,
            TenantIdentifier = _faker.Random.Guid()
        };

        _aggregateCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(aggregateDataModel);

        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        AggregateDataModel result = aggregateStore.Get(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(aggregateDataModel);
    }

    [Test]
    public async Task GetAsync_WhenAggregateIdentifierIsProvided_ShouldReturnAggregate()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();

        AggregateDataModel aggregateDataModel = new()
        {
            AggregateIdentifier = _faker.Random.Guid(),
            AggregateVersion = _faker.Random.Int(),
            DeletedDate = _faker.Date.Future(),
            IsDeleted = true,
            TenantIdentifier = _faker.Random.Guid()
        };

        _aggregateCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(aggregateDataModel);

        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        AggregateDataModel result = await aggregateStore.GetAsync(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(aggregateDataModel);
    }

    [Test]
    public void Get_WhenAggregateIdentifierIsProvided_ShouldReturnDomainEvents()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        List<DomainEventDataModel> domainEventsDms = GenerateDomainEventsCollection(aggregateIdentifier);

        _eventsCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()))
            .Returns(domainEventsDms);
        
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        IReadOnlyCollection<DomainEventDataModel> result = aggregateStore.Get(aggregateIdentifier, aggregateVersion);

        // Assert
        result.Should().BeEquivalentTo(domainEventsDms);
    }

    [Test]
    public async Task GetAsync_WhenAggregateIdentifierIsProvided_ShouldReturnDomainEvents()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        int aggregateVersion = _faker.Random.Int();
        List<DomainEventDataModel> domainEventsDms = GenerateDomainEventsCollection(aggregateIdentifier);

        _eventsCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()))
            .Returns(domainEventsDms);

        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        IReadOnlyCollection<DomainEventDataModel> result =
            await aggregateStore.GetAsync(aggregateIdentifier, aggregateVersion);

        // Assert
        result.Should().BeEquivalentTo(domainEventsDms);
    }

    [Test]
    public void GetDeleted_WhenAggregateIdentifierIsProvided_ShouldReturnDomainEvents()
    {
        // Arrange
        List<AggregateDataModel> aggregates = GenerateAggregates();
        _aggregateCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, AggregateDataModel, bool>>())).Returns(aggregates);
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        IEnumerable<Guid> result = aggregateStore.GetDeleted();

        // Assert
        result.Should().BeEquivalentTo(aggregates.Select(a => a.AggregateIdentifier));
    }

    [Test]
    public async Task GetDeletedAsync_WhenAggregateIdentifierIsProvided_ShouldReturnDomainEvents()
    {
        // Arrange
        List<AggregateDataModel> aggregates = GenerateAggregates();
        _aggregateCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, AggregateDataModel, bool>>())).Returns(aggregates);
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        IEnumerable<Guid> result = await aggregateStore.GetDeletedAsync();

        // Assert
        result.Should().BeEquivalentTo(aggregates.Select(a => a.AggregateIdentifier));
    }

    [Test]
    public void Save_WhenDomainEventsAreProvided_ShouldSaveDomainEvents()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion,
            _identityServiceMock.Object.GetCurrent());

        AggregateDataModel aggregateDataModel = new()
        {
            AggregateIdentifier = aggregateIdentifier,
            AggregateVersion = aggregateVersion,
            DeletedDate = aggregate.DeletedDate,
            IsDeleted = aggregate.IsDeleted,
            TenantIdentifier = aggregate.IdentityContext?.Tenant?.Identifier ?? Guid.Empty
        };

        List<DomainEventDataModel> domainEventsDms = GenerateDomainEventsCollection(aggregateIdentifier);
        IEnumerable<KeyValuePair<Guid, DomainEventDataModel>> domainEventsDictionary =
            domainEventsDms?.Select(v => new KeyValuePair<Guid, DomainEventDataModel>(v.EventIdentifier, v));

        _aggregateCacheMock.Setup(x => x.Add(aggregateIdentifier, aggregateDataModel));

        _eventsCacheMock.Setup(x => x.AddMany(domainEventsDictionary));

        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        aggregateStore.Save(aggregateDataModel, domainEventsDms);

        // Assert
        _aggregateCacheMock.Verify(x => x.Add(aggregateIdentifier, aggregateDataModel), Times.Once);
        _eventsCacheMock.Verify(x => x.AddMany(domainEventsDictionary), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenDomainEventsAreProvided_ShouldSaveDomainEvents()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        int aggregateVersion = _faker.Random.Int();
        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion,
            _identityServiceMock.Object.GetCurrent());

        AggregateDataModel aggregateDataModel = new()
        {
            AggregateIdentifier = aggregateIdentifier,
            AggregateVersion = aggregateVersion,
            DeletedDate = aggregate.DeletedDate,
            IsDeleted = aggregate.IsDeleted,
            TenantIdentifier = aggregate.IdentityContext?.Tenant?.Identifier ?? Guid.Empty
        };

        List<DomainEventDataModel> domainEventsDms = GenerateDomainEventsCollection(aggregateIdentifier);
        IEnumerable<KeyValuePair<Guid, DomainEventDataModel>> domainEventsDictionary =
            domainEventsDms?.Select(v => new KeyValuePair<Guid, DomainEventDataModel>(v.EventIdentifier, v));

        _aggregateCacheMock.Setup(x => x.Add(aggregateIdentifier, aggregateDataModel));

        _eventsCacheMock.Setup(x => x.AddMany(domainEventsDictionary));

        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        await aggregateStore.SaveAsync(aggregateDataModel, domainEventsDms);

        // Assert
        _aggregateCacheMock.Verify(x => x.Add(aggregateIdentifier, aggregateDataModel), Times.Once);
        _eventsCacheMock.Verify(x => x.AddMany(domainEventsDictionary), Times.Once);
    }

    [Test]
    public void Box_WhenAggregateIsProvided_ShouldReturnAggregateBox()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion,
            _identityServiceMock.Object.GetCurrent());

        AggregateDataModel aggregateDataModel = new()
        {
            AggregateIdentifier = aggregateIdentifier,
            AggregateVersion = aggregateVersion,
            DeletedDate = aggregate.DeletedDate,
            IsDeleted = aggregate.IsDeleted,
            TenantIdentifier = aggregate.IdentityContext?.Tenant?.Identifier ?? Guid.Empty
        };

        List<DomainEventDataModel> domainEventsDms = GenerateDomainEventsCollection(aggregateIdentifier);

        _aggregateCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(aggregateDataModel);
        _eventsCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()))
            .Returns(domainEventsDms);
        _aggregateOfflineStorageMock.Setup(x => x.Save(aggregateDataModel, domainEventsDms));

        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        aggregateStore.Box(aggregateIdentifier);

        // Assert
        _aggregateCacheMock.Verify(x => x.Get(aggregateIdentifier), Times.Once);
        _aggregateOfflineStorageMock.Verify(x => x.Save(aggregateDataModel, domainEventsDms), Times.Once);
        _eventsCacheMock.Verify(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()), Times.Once);
    }

    [Test]
    public async Task BoxAsync_WhenAggregateIsProvided_ShouldReturnAggregateBox()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion,
            _identityServiceMock.Object.GetCurrent());

        AggregateDataModel aggregateDataModel = new()
        {
            AggregateIdentifier = aggregateIdentifier,
            AggregateVersion = aggregateVersion,
            DeletedDate = aggregate.DeletedDate,
            IsDeleted = aggregate.IsDeleted,
            TenantIdentifier = aggregate.IdentityContext?.Tenant?.Identifier ?? Guid.Empty
        };

        List<DomainEventDataModel> domainEventsDms = GenerateDomainEventsCollection(aggregateIdentifier);

        _aggregateCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(aggregateDataModel);
        _eventsCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()))
            .Returns(domainEventsDms);
        _aggregateOfflineStorageMock.Setup(x => x.SaveAsync(aggregateDataModel, domainEventsDms, default));

        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        await aggregateStore.BoxAsync(aggregateIdentifier);

        // Assert
        _aggregateCacheMock.Verify(x => x.Get(aggregateIdentifier), Times.Once);
        _aggregateOfflineStorageMock.Verify(x => x.SaveAsync(aggregateDataModel, domainEventsDms, default), Times.Once);
        _eventsCacheMock.Verify(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()), Times.Once);
    }

    private List<AggregateDataModel> GenerateAggregates()
    {
        List<AggregateRoot> aggregates = new()
        {
            GenericAggregateFactory<Post>.CreateAggregate(_faker.Random.Guid(), _faker.Random.Int(1, 1000),
                _identityServiceMock.Object.GetCurrent()),
            GenericAggregateFactory<Post>.CreateAggregate(_faker.Random.Guid(), _faker.Random.Int(1, 1000),
                _identityServiceMock.Object.GetCurrent()),
            GenericAggregateFactory<Post>.CreateAggregate(_faker.Random.Guid(), _faker.Random.Int(1, 1000),
                _identityServiceMock.Object.GetCurrent())
        };

        List<AggregateDataModel> aggregateDataModels = aggregates.Select(a => new AggregateDataModel
        {
            AggregateIdentifier = a.AggregateIdentifier,
            AggregateVersion = a.AggregateVersion,
            DeletedDate = a.DeletedDate,
            IsDeleted = a.IsDeleted,
            TenantIdentifier = a.IdentityContext?.Tenant?.Identifier ?? Guid.Empty
        }).ToList();

        return aggregateDataModels;
    }

    private List<DomainEventDataModel> GenerateDomainEventsCollection(Guid aggregateIdentifier)
    {
        List<IDomainEvent> domainEvents = new()
        {
            new ChangeDescriptionDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                Description.Create(_faker.Lorem.Sentences())),
            new ChangeTitleDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                Title.Default(_faker.Lorem.Sentence())),
            new ChangeDescriptionDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                Description.Create(_faker.Lorem.Sentences())),
            new ChangeTitleDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                Title.Default(_faker.Lorem.Sentence())),
            new ChangeTitleDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                Title.Default(_faker.Lorem.Sentence())),
            new AddImagesDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                new HashSet<Image>
                {
                    Image.Create(_faker.Random.Guid(),
                        Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150))),
                        BytesContent.Create(_faker.Random.Bytes(389)))
                }),
            new ChangeDescriptionDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                Description.Create(_faker.Lorem.Sentences())),
            new AddImagesDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                new HashSet<Image>
                {
                    Image.Create(_faker.Random.Guid(),
                        Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150))),
                        BytesContent.Create(_faker.Random.Bytes(2346))),
                    Image.Create(_faker.Random.Guid(),
                        Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150))),
                        BytesContent.Create(_faker.Random.Bytes(982)))
                }),
            new ChangeImageNameDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                _faker.Random.Guid(), Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150)))),
            new ChangeExpirationDateDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                Date.Create(_faker.Date.Future()))
        };

        List<DomainEventDataModel> domainEventDms = domainEvents.Select(e => new DomainEventDataModel
        {
            AggregateIdentifier = e.AggregateIdentifier,
            AggregateVersion = e.AggregateVersion,
            EventIdentifier = e.EventIdentifier,
            EventType = e.GetTypeFullName(),
            EventClass = e.GetClassName(),
            DomainEvent = e,
            CreatedAt = e.EventTime
        }).ToList();

        return _faker.Random.ListItems(domainEventDms, _faker.Random.Int(1, 10)).ToList();
    }
}