﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.Events.Services.Interfaces;
using EssentialFrame.Domain.EventSourcing.Core.Factories;
using EssentialFrame.Domain.EventSourcing.Exceptions;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Persistence.Mappers;
using EssentialFrame.Domain.Persistence.Mappers.Interfaces;
using EssentialFrame.Domain.Persistence.Models;
using EssentialFrame.ExampleApp.Application.Identity;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents.Events;
using EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.BytesContents;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;
using EssentialFrame.Extensions;
using EssentialFrame.Identity.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.EventSourcing.Tests.UnitTests.Persistence.Aggregates;

[TestFixture]
public class EventSourcingAggregateRepositoryTests
{
    private readonly Faker _faker = new();
    private readonly Mock<IEventSourcingAggregateMapper<Post, PostIdentifier, Guid>> _aggregateMapperMock = new();
    private readonly Mock<IDomainEventMapper<PostIdentifier, Guid>> _domainEventMapperMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly Mock<IEventSourcingAggregateStore> _aggregateStoreMock = new();
    private readonly Mock<IDomainEventsPublisher<PostIdentifier, Guid>> _domainEventsPublisherMock = new();
    private readonly CancellationToken _cancellationToken = default;

    [SetUp]
    public void SetUp()
    {
        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new AppIdentityContext());
    }

    [TearDown]
    public void TearDown()
    {
        _domainEventMapperMock.Reset();
        _identityServiceMock.Reset();
        _aggregateStoreMock.Reset();
        _aggregateMapperMock.Reset();
        _domainEventsPublisherMock.Reset();
    }

    [Test]
    public void Get_WhenAggregateExists_ReturnsAggregate()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        EventSourcingAggregateDataModel eventSourcingAggregateDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier,
            AggregateVersion = aggregate.AggregateVersion,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent().Tenant.Identifier
        };

        List<DomainEventDataModel> eventDataModels = GenerateDomainEventDataModelsCollection(aggregateIdentifier);

        aggregate.Rehydrate(GenerateDomainEventsCollection(aggregateIdentifier));

        _aggregateStoreMock.Setup(x => x.Get(aggregateIdentifier, -1)).Returns(eventDataModels);
        _domainEventMapperMock.Setup(x => x.Map(eventDataModels))
            .Returns(GenerateDomainEventsCollection(aggregateIdentifier));
        _aggregateStoreMock.Setup(x => x.Get(aggregateIdentifier)).Returns(eventSourcingAggregateDataModel);

        IEventSourcingAggregateRepository<Post, PostIdentifier, Guid> eventSourcingAggregateRepository =
            new EventSourcingAggregateRepository<Post, PostIdentifier, Guid>(_aggregateStoreMock.Object,
                _domainEventMapperMock.Object, _aggregateMapperMock.Object, _domainEventsPublisherMock.Object);

        // Act
        Post result = eventSourcingAggregateRepository.Get(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(aggregate);

        _aggregateStoreMock.Verify(x => x.Get(aggregateIdentifier, -1), Times.Once);
        _domainEventMapperMock.Verify(x => x.Map(eventDataModels), Times.Once);
        _aggregateStoreMock.Verify(x => x.Get(aggregateIdentifier), Times.Once);
    }

    [Test]
    public void Get_WhenAggregateEventsNotFound_ShouldThrowAggregateNotFoundException()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        EventSourcingAggregateDataModel eventSourcingAggregateDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier,
            AggregateVersion = aggregate.AggregateVersion,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent().Tenant.Identifier
        };

        List<IDomainEvent<PostIdentifier, Guid>> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _aggregateStoreMock.Setup(x => x.Get(aggregateIdentifier, -1))
            .Returns((IReadOnlyCollection<DomainEventDataModel>)null);

        _aggregateStoreMock.Setup(x => x.Get(aggregateIdentifier)).Returns(eventSourcingAggregateDataModel);

        IEventSourcingAggregateRepository<Post, PostIdentifier, Guid> eventSourcingAggregateRepository =
            new EventSourcingAggregateRepository<Post, PostIdentifier, Guid>(_aggregateStoreMock.Object,
                _domainEventMapperMock.Object, _aggregateMapperMock.Object, _domainEventsPublisherMock.Object);

        // Act
        Action getAggregateAction = () => eventSourcingAggregateRepository.Get(aggregateIdentifier);

        // Assert
        getAggregateAction.Should().Throw<AggregateNotFoundException>().WithMessage(
            $"Aggregate ({aggregate.GetTypeFullName()}) with identifier: ({aggregateIdentifier}) hasn't been found");

        _aggregateStoreMock.Verify(x => x.Get(aggregateIdentifier, -1), Times.Once);
        _aggregateStoreMock.Verify(x => x.Get(aggregateIdentifier), Times.Once);
    }

    [Test]
    public async Task GetAsync_WhenAggregateExists_ReturnsAggregate()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        EventSourcingAggregateDataModel eventSourcingAggregateDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier,
            AggregateVersion = aggregate.AggregateVersion,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent().Tenant.Identifier
        };

        List<DomainEventDataModel> eventDataModels = GenerateDomainEventDataModelsCollection(aggregateIdentifier);

        aggregate.Rehydrate(GenerateDomainEventsCollection(aggregateIdentifier));

        _aggregateStoreMock.Setup(x => x.GetAsync(aggregateIdentifier, -1, _cancellationToken))
            .ReturnsAsync(eventDataModels);
        _aggregateStoreMock.Setup(x => x.GetAsync(aggregateIdentifier, _cancellationToken))
            .ReturnsAsync(eventSourcingAggregateDataModel);
        _domainEventMapperMock.Setup(x => x.Map(eventDataModels))
            .Returns(GenerateDomainEventsCollection(aggregateIdentifier));

        IEventSourcingAggregateRepository<Post, PostIdentifier, Guid> eventSourcingAggregateRepository =
            new EventSourcingAggregateRepository<Post, PostIdentifier, Guid>(_aggregateStoreMock.Object,
                _domainEventMapperMock.Object, _aggregateMapperMock.Object, _domainEventsPublisherMock.Object);

        // Act
        Post result = await eventSourcingAggregateRepository.GetAsync(aggregateIdentifier, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(aggregate);

        _aggregateStoreMock.Verify(x => x.GetAsync(aggregateIdentifier, -1, _cancellationToken), Times.Once);
        _domainEventMapperMock.Verify(x => x.Map(eventDataModels), Times.Once);
        _aggregateStoreMock.Verify(x => x.GetAsync(aggregateIdentifier, _cancellationToken), Times.Once);
    }

    [Test]
    public async Task GetAsync_WhenAggregateEventsNotFound_ShouldThrowAggregateNotFoundException()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        EventSourcingAggregateDataModel eventSourcingAggregateDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier,
            AggregateVersion = aggregate.AggregateVersion,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent().Tenant.Identifier
        };

        List<IDomainEvent<PostIdentifier, Guid>> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _aggregateStoreMock.Setup(x => x.GetAsync(aggregateIdentifier, -1, _cancellationToken))
            .ReturnsAsync((IReadOnlyCollection<DomainEventDataModel>)null);
        _aggregateStoreMock.Setup(x => x.GetAsync(aggregateIdentifier, _cancellationToken))
            .ReturnsAsync(eventSourcingAggregateDataModel);

        IEventSourcingAggregateRepository<Post, PostIdentifier, Guid> eventSourcingAggregateRepository =
            new EventSourcingAggregateRepository<Post, PostIdentifier, Guid>(_aggregateStoreMock.Object,
                _domainEventMapperMock.Object, _aggregateMapperMock.Object, _domainEventsPublisherMock.Object);

        // Act
        Func<Task> getAggregateAction = async () =>
            await eventSourcingAggregateRepository.GetAsync(aggregateIdentifier, _cancellationToken);

        // Assert
        await getAggregateAction.Should().ThrowAsync<AggregateNotFoundException>().WithMessage(
            $"Aggregate ({aggregate.GetTypeFullName()}) with identifier: ({aggregateIdentifier}) hasn't been found");

        _aggregateStoreMock.Verify(x => x.GetAsync(aggregateIdentifier, -1, _cancellationToken), Times.Once);
        _aggregateStoreMock.Verify(x => x.GetAsync(aggregateIdentifier, _cancellationToken), Times.Once);
    }

    [Test]
    public void Save_WhenCorrectAggregateProvided_ShouldSaveAggregate()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        aggregate.Create(Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
            Date.Create(_faker.Date.FutureOffset()), null, _identityServiceMock.Object.GetCurrent());

        aggregate.ChangeDescription(Description.Create(_faker.Lorem.Sentences()),
            _identityServiceMock.Object.GetCurrent());
        aggregate.ChangeTitle(Title.Default(_faker.Lorem.Sentence()), _identityServiceMock.Object.GetCurrent());
        aggregate.ExtendExpirationDate(Date.Create(_faker.Date.FutureOffset()),
            _identityServiceMock.Object.GetCurrent());

        EventSourcingAggregateDataModel eventSourcingAggregateDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier,
            AggregateVersion = aggregate.AggregateVersion,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent().Tenant.Identifier
        };

        IDomainEvent<PostIdentifier, Guid>[] events = aggregate.GetUncommittedChanges();
        List<DomainEventDataModel> eventDataModels =
            GenerateDomainEventDataModelsCollection(aggregateIdentifier, events);

        _aggregateMapperMock.Setup(x => x.Map(aggregate)).Returns(eventSourcingAggregateDataModel);
        _domainEventMapperMock.Setup(x => x.Map(events)).Returns(eventDataModels);
        _aggregateStoreMock.Setup(x => x.Save(eventSourcingAggregateDataModel, eventDataModels));

        IEventSourcingAggregateRepository<Post, PostIdentifier, Guid> eventSourcingAggregateRepository =
            new EventSourcingAggregateRepository<Post, PostIdentifier, Guid>(_aggregateStoreMock.Object,
                _domainEventMapperMock.Object, _aggregateMapperMock.Object, _domainEventsPublisherMock.Object);

        // Act
        IDomainEvent<PostIdentifier, Guid>[] domainEvents = eventSourcingAggregateRepository.Save(aggregate);

        // Assert
        domainEvents.Should().BeEquivalentTo(events);
        _aggregateMapperMock.Verify(x => x.Map(aggregate), Times.Once());
        _domainEventMapperMock.Verify(x => x.Map(events), Times.Once());
        _aggregateStoreMock.Verify(x => x.Save(eventSourcingAggregateDataModel, eventDataModels), Times.Once());
    }

    [Test]
    public void Save_WhenNextVersionIsOccupied_ShouldThrowConcurrencyException()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;
        const int expectedAggregateVersion = 0;

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        aggregate.Create(Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
            Date.Create(_faker.Date.FutureOffset()), null, _identityServiceMock.Object.GetCurrent());

        aggregate.ChangeDescription(Description.Create(_faker.Lorem.Sentences()),
            _identityServiceMock.Object.GetCurrent());
        aggregate.ChangeTitle(Title.Default(_faker.Lorem.Sentence()), _identityServiceMock.Object.GetCurrent());
        aggregate.ExtendExpirationDate(Date.Create(_faker.Date.FutureOffset()),
            _identityServiceMock.Object.GetCurrent());

        _aggregateStoreMock.Setup(x => x.Exists(aggregateIdentifier, expectedAggregateVersion)).Returns(true);

        IEventSourcingAggregateRepository<Post, PostIdentifier, Guid> eventSourcingAggregateRepository =
            new EventSourcingAggregateRepository<Post, PostIdentifier, Guid>(_aggregateStoreMock.Object,
                _domainEventMapperMock.Object, _aggregateMapperMock.Object, _domainEventsPublisherMock.Object);

        // Act
        Action saveAction = () => eventSourcingAggregateRepository.Save(aggregate, expectedAggregateVersion);

        // Assert
        saveAction.Should().Throw<ConcurrencyException>().WithMessage(
            $"A concurrency violation occurred on this aggregate ({aggregateIdentifier}). At least one event failed to save");

        _aggregateStoreMock.Verify(x => x.Exists(aggregateIdentifier, expectedAggregateVersion), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenCorrectAggregateProvided_ShouldSaveAggregate()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        aggregate.Create(Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
            Date.Create(_faker.Date.FutureOffset()), null, _identityServiceMock.Object.GetCurrent());

        aggregate.ChangeDescription(Description.Create(_faker.Lorem.Sentences()),
            _identityServiceMock.Object.GetCurrent());
        aggregate.ChangeTitle(Title.Default(_faker.Lorem.Sentence()), _identityServiceMock.Object.GetCurrent());
        aggregate.ExtendExpirationDate(Date.Create(_faker.Date.FutureOffset()),
            _identityServiceMock.Object.GetCurrent());

        EventSourcingAggregateDataModel eventSourcingAggregateDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier,
            AggregateVersion = aggregate.AggregateVersion,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent().Tenant.Identifier
        };

        IDomainEvent<PostIdentifier, Guid>[] events = aggregate.GetUncommittedChanges();
        List<DomainEventDataModel> eventDataModels =
            GenerateDomainEventDataModelsCollection(aggregateIdentifier, events);

        _aggregateMapperMock.Setup(x => x.Map(aggregate)).Returns(eventSourcingAggregateDataModel);
        _domainEventMapperMock.Setup(x => x.Map(events)).Returns(eventDataModels);
        _aggregateStoreMock.Setup(
            x => x.SaveAsync(eventSourcingAggregateDataModel, eventDataModels, _cancellationToken));

        IEventSourcingAggregateRepository<Post, PostIdentifier, Guid> eventSourcingAggregateRepository =
            new EventSourcingAggregateRepository<Post, PostIdentifier, Guid>(_aggregateStoreMock.Object,
                _domainEventMapperMock.Object, _aggregateMapperMock.Object, _domainEventsPublisherMock.Object);

        // Act
        IDomainEvent<PostIdentifier, Guid>[] domainEvents =
            await eventSourcingAggregateRepository.SaveAsync(aggregate, _cancellationToken);

        // Assert
        domainEvents.Should().BeEquivalentTo(events);
        _aggregateMapperMock.Verify(x => x.Map(aggregate), Times.Once());
        _domainEventMapperMock.Verify(x => x.Map(events), Times.Once());
        _aggregateStoreMock.Verify(
            x => x.SaveAsync(eventSourcingAggregateDataModel, eventDataModels, _cancellationToken),
            Times.Once());
    }

    [Test]
    public async Task SaveAsync_WhenNextVersionIsOccupied_ShouldThrowConcurrencyException()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;
        const int expectedAggregateVersion = 0;

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        aggregate.Create(Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
            Date.Create(_faker.Date.FutureOffset()), null, _identityServiceMock.Object.GetCurrent());

        aggregate.ChangeDescription(Description.Create(_faker.Lorem.Sentences()),
            _identityServiceMock.Object.GetCurrent());
        aggregate.ChangeTitle(Title.Default(_faker.Lorem.Sentence()), _identityServiceMock.Object.GetCurrent());
        aggregate.ExtendExpirationDate(Date.Create(_faker.Date.FutureOffset()),
            _identityServiceMock.Object.GetCurrent());

        _aggregateStoreMock.Setup(x => x.ExistsAsync(aggregateIdentifier, expectedAggregateVersion, _cancellationToken))
            .ReturnsAsync(true);

        IEventSourcingAggregateRepository<Post, PostIdentifier, Guid> eventSourcingAggregateRepository =
            new EventSourcingAggregateRepository<Post, PostIdentifier, Guid>(_aggregateStoreMock.Object,
                _domainEventMapperMock.Object, _aggregateMapperMock.Object, _domainEventsPublisherMock.Object);

        // Act
        Func<Task> saveAction = async () =>
            await eventSourcingAggregateRepository.SaveAsync(aggregate, expectedAggregateVersion, _cancellationToken);

        // Assert
        await saveAction.Should().ThrowAsync<ConcurrencyException>().WithMessage(
            $"A concurrency violation occurred on this aggregate ({aggregateIdentifier}). At least one event failed to save");

        _aggregateStoreMock.Verify(
            x => x.ExistsAsync(aggregateIdentifier, expectedAggregateVersion, _cancellationToken),
            Times.Once);
    }

    private List<IDomainEvent<PostIdentifier, Guid>> GenerateDomainEventsCollection(PostIdentifier aggregateIdentifier)
    {
        Guid imageId = _faker.Random.Guid();

        List<IDomainEvent<PostIdentifier, Guid>> domainEvents = new()
        {
            new NewPostCreatedDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(), 1,
                Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
                Date.Create(_faker.Date.FutureOffset()), null),
            new DescriptionChangedDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(), 2,
                Description.Create(_faker.Lorem.Sentences())),
            new ImagesAddedDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(), 3,
                new HashSet<Image>
                {
                    Image.Create(imageId, Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150))),
                        BytesContent.Create(_faker.Random.Bytes(2346))),
                    Image.Create(_faker.Random.Guid(),
                        Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150))),
                        BytesContent.Create(_faker.Random.Bytes(982)))
                }),
            new ImageNameChangedDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(), 4,
                imageId, Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150)))),
            new ExpirationChangedDateDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(), 5,
                Date.Create(_faker.Date.Future()))
        };

        return domainEvents;
    }

    private List<DomainEventDataModel> GenerateDomainEventDataModelsCollection(PostIdentifier aggregateIdentifier,
        IEnumerable<IDomainEvent<PostIdentifier, Guid>> events = null)
    {
        IDomainEventMapper<PostIdentifier, Guid> domainEventMapper = new DomainEventMapper<PostIdentifier, Guid>();

        List<IDomainEvent<PostIdentifier, Guid>> domainEvents =
            events?.ToList() ?? GenerateDomainEventsCollection(aggregateIdentifier);
        
        List<DomainEventDataModel> domainEventDms = domainEventMapper.Map(domainEvents).ToList();

        return domainEventDms;
    }
}