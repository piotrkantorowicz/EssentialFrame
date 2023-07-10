using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Interfaces;
using EssentialFrame.Domain.Events.Persistence.DomainEvents;
using EssentialFrame.Domain.Events.Persistence.DomainEvents.Interfaces;
using EssentialFrame.Domain.Factories;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;
using EssentialFrame.ExampleApp.Domain.Posts.Entities;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects;
using EssentialFrame.ExampleApp.Identity;
using EssentialFrame.Extensions;
using EssentialFrame.Identity;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.Events.Tests.UnitTests.Events;

[TestFixture]
public class DefaultDomainEventStoreTests
{
    [SetUp]
    public void SetUp()
    {
        _aggregateCacheMock = new Mock<ICache<Guid, AggregateRoot>>();
        _eventsCacheMock = new Mock<ICache<Guid, DomainEventDataModel>>();
        _identityServiceMock = new Mock<IIdentityService>();
        _aggregateOfflineStorageMock = new Mock<IAggregateOfflineStorage>();
        _domainEventMapper = new Mock<IDomainEventMapper>();

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
    }

    private readonly Faker _faker = new();
    private Mock<ICache<Guid, AggregateRoot>> _aggregateCacheMock;
    private Mock<ICache<Guid, DomainEventDataModel>> _eventsCacheMock;
    private Mock<IIdentityService> _identityServiceMock;
    private Mock<IAggregateOfflineStorage> _aggregateOfflineStorageMock;
    private Mock<IDomainEventMapper> _domainEventMapper;

    [Test]
    public void Exists_WhenAggregateIdentifierIsProvided_ShouldReturnTrue()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        _aggregateCacheMock.Setup(x => x.Exists(aggregateIdentifier)).Returns(true);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = domainEventStore.Exists(aggregateIdentifier);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task ExistsAsync_WhenAggregateIdentifierIsProvided_ShouldReturnTrue()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        _aggregateCacheMock.Setup(x => x.Exists(aggregateIdentifier)).Returns(true);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = await domainEventStore.ExistsAsync(aggregateIdentifier);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Exists_WhenAggregateIdentifierIsProvided_ShouldReturnFalse()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        _aggregateCacheMock.Setup(x => x.Exists(aggregateIdentifier)).Returns(false);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = domainEventStore.Exists(aggregateIdentifier);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task ExistsAsync_WhenAggregateIdentifierIsProvided_ShouldReturnFalse()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        _aggregateCacheMock.Setup(x => x.Exists(aggregateIdentifier)).Returns(false);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = await domainEventStore.ExistsAsync(aggregateIdentifier);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Exists_WhenAggregateIdentifierAndVersionAreProvided_ShouldReturnTrue()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        _aggregateCacheMock.Setup(x => x.Exists(It.IsAny<Func<Guid, AggregateRoot, bool>>())).Returns(true);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = domainEventStore.Exists(aggregateIdentifier, aggregateVersion);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task ExistsAsync_WhenAggregateIdentifierAndVersionAreProvided_ShouldReturnTrue()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        int aggregateVersion = _faker.Random.Int();
        _aggregateCacheMock.Setup(x => x.Exists(It.IsAny<Func<Guid, AggregateRoot, bool>>())).Returns(true);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = await domainEventStore.ExistsAsync(aggregateIdentifier, aggregateVersion);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Exists_WhenAggregateIdentifierAndVersionAreProvided_ShouldReturnFalse()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        _aggregateCacheMock.Setup(x => x.Exists(It.IsAny<Func<Guid, AggregateRoot, bool>>())).Returns(false);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = domainEventStore.Exists(aggregateIdentifier, aggregateVersion);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task ExistsAsync_WhenAggregateIdentifierAndVersionAreProvided_ShouldReturnFalse()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        int aggregateVersion = _faker.Random.Int();
        _aggregateCacheMock.Setup(x => x.Exists(It.IsAny<Func<Guid, AggregateRoot, bool>>())).Returns(false);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = await domainEventStore.ExistsAsync(aggregateIdentifier, aggregateVersion);

        // Assert
        result.Should().BeFalse();
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
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        IReadOnlyCollection<DomainEventDataModel> result = domainEventStore.Get(aggregateIdentifier, aggregateVersion);

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

        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        IReadOnlyCollection<DomainEventDataModel> result =
            await domainEventStore.GetAsync(aggregateIdentifier, aggregateVersion);

        // Assert
        result.Should().BeEquivalentTo(domainEventsDms);
    }

    [Test]
    public void GetDeleted_WhenAggregateIdentifierIsProvided_ShouldReturnDomainEvents()
    {
        // Arrange
        List<AggregateRoot> aggregates = GenerateAggregates();
        _aggregateCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, AggregateRoot, bool>>())).Returns(aggregates);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        IEnumerable<Guid> result = domainEventStore.GetDeleted();

        // Assert
        result.Should().BeEquivalentTo(aggregates.Select(a => a.AggregateIdentifier));
    }

    [Test]
    public async Task GetDeletedAsync_WhenAggregateIdentifierIsProvided_ShouldReturnDomainEvents()
    {
        // Arrange
        List<AggregateRoot> aggregates = GenerateAggregates();
        _aggregateCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, AggregateRoot, bool>>())).Returns(aggregates);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        IEnumerable<Guid> result = await domainEventStore.GetDeletedAsync();

        // Assert
        result.Should().BeEquivalentTo(aggregates.Select(a => a.AggregateIdentifier));
    }

    [Test]
    public void Save_WhenDomainEventsAreProvided_ShouldSaveDomainEvents()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion);
        List<DomainEventDataModel> domainEventsDms = GenerateDomainEventsCollection(aggregateIdentifier);
        IEnumerable<KeyValuePair<Guid, DomainEventDataModel>> domainEventsDictionary =
            domainEventsDms?.Select(v => new KeyValuePair<Guid, DomainEventDataModel>(v.EventIdentifier, v));

        _aggregateCacheMock.Setup(x => x.Add(aggregateIdentifier, aggregate));

        _eventsCacheMock.Setup(x => x.AddMany(domainEventsDictionary));

        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        domainEventStore.Save(aggregate, domainEventsDms);

        // Assert
        _aggregateCacheMock.Verify(x => x.Add(aggregateIdentifier, aggregate), Times.Once);
        _eventsCacheMock.Verify(x => x.AddMany(domainEventsDictionary), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenDomainEventsAreProvided_ShouldSaveDomainEvents()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        int aggregateVersion = _faker.Random.Int();
        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion);
        List<DomainEventDataModel> domainEventsDms = GenerateDomainEventsCollection(aggregateIdentifier);
        IEnumerable<KeyValuePair<Guid, DomainEventDataModel>> domainEventsDictionary =
            domainEventsDms?.Select(v => new KeyValuePair<Guid, DomainEventDataModel>(v.EventIdentifier, v));

        _aggregateCacheMock.Setup(x => x.Add(aggregateIdentifier, aggregate));

        _eventsCacheMock.Setup(x => x.AddMany(domainEventsDictionary));

        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        await domainEventStore.SaveAsync(aggregate, domainEventsDms);

        // Assert
        _aggregateCacheMock.Verify(x => x.Add(aggregateIdentifier, aggregate), Times.Once);
        _eventsCacheMock.Verify(x => x.AddMany(domainEventsDictionary), Times.Once);
    }

    [Test]
    public void Box_WhenAggregateIsProvided_ShouldReturnAggregateBox()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion);
        List<DomainEventDataModel> domainEventsDms = GenerateDomainEventsCollection(aggregateIdentifier);

        _aggregateCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(aggregate);
        _eventsCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()))
            .Returns(domainEventsDms);
        _aggregateOfflineStorageMock.Setup(x => x.Save(aggregate, domainEventsDms));

        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        domainEventStore.Box(aggregateIdentifier);

        // Assert
        _aggregateCacheMock.Verify(x => x.Get(aggregateIdentifier), Times.Once);
        _aggregateOfflineStorageMock.Verify(x => x.Save(aggregate, domainEventsDms), Times.Once);
        _eventsCacheMock.Verify(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()), Times.Once);
    }

    [Test]
    public async Task BoxAsync_WhenAggregateIsProvided_ShouldReturnAggregateBox()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion);
        List<DomainEventDataModel> domainEventsDms = GenerateDomainEventsCollection(aggregateIdentifier);

        _aggregateCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(aggregate);
        _eventsCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()))
            .Returns(domainEventsDms);
        _aggregateOfflineStorageMock.Setup(x => x.SaveAsync(aggregate, domainEventsDms, default));

        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        await domainEventStore.BoxAsync(aggregateIdentifier);

        // Assert
        _aggregateCacheMock.Verify(x => x.Get(aggregateIdentifier), Times.Once);
        _aggregateOfflineStorageMock.Verify(x => x.SaveAsync(aggregate, domainEventsDms, default), Times.Once);
        _eventsCacheMock.Verify(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()), Times.Once);
    }

    private List<AggregateRoot> GenerateAggregates()
    {
        List<AggregateRoot> aggregates = new()
        {
            GenericAggregateFactory<Post>.CreateAggregate(_faker.Random.Guid(), _faker.Random.Int(1, 1000)),
            GenericAggregateFactory<Post>.CreateAggregate(_faker.Random.Guid(), _faker.Random.Int(1, 1000)),
            GenericAggregateFactory<Post>.CreateAggregate(_faker.Random.Guid(), _faker.Random.Int(1, 1000))
        };

        return aggregates;
    }

    private List<DomainEventDataModel> GenerateDomainEventsCollection(Guid aggregateIdentifier)
    {
        IIdentityContext identityContext = _identityServiceMock.Object.GetCurrent();

        List<IDomainEvent> domainEvents = new()
        {
            new ChangeDescriptionDomainEvent(aggregateIdentifier, identityContext, _faker.Lorem.Sentences()),
            new ChangeTitleDomainEvent(aggregateIdentifier, identityContext,
                Title.Create(_faker.Random.Word(), _faker.Random.Bool())),
            new ChangeDescriptionDomainEvent(aggregateIdentifier, identityContext, _faker.Lorem.Sentences()),
            new ChangeTitleDomainEvent(aggregateIdentifier, identityContext,
                Title.Create(_faker.Random.Word(), _faker.Random.Bool())),
            new ChangeTitleDomainEvent(aggregateIdentifier, identityContext,
                Title.Create(_faker.Random.Word(), _faker.Random.Bool())),
            new AddImagesDomainEvent(aggregateIdentifier, identityContext,
                new HashSet<Image>
                {
                    Image.Create(_faker.Random.Guid(), _faker.Random.Word(), _faker.Random.Bytes(389))
                }),
            new ChangeDescriptionDomainEvent(aggregateIdentifier, identityContext, _faker.Lorem.Sentences()),
            new AddImagesDomainEvent(aggregateIdentifier, identityContext,
                new HashSet<Image>
                {
                    Image.Create(_faker.Random.Guid(), _faker.Random.Word(), _faker.Random.Bytes(2346)),
                    Image.Create(_faker.Random.Guid(), _faker.Random.Word(), _faker.Random.Bytes(982))
                }),
            new ChangeImageNameDomainEvent(aggregateIdentifier, identityContext, _faker.Random.Guid(),
                _faker.Lorem.Word()),
            new ChangeExpirationDateDomainEvent(aggregateIdentifier, identityContext, _faker.Date.Future())
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