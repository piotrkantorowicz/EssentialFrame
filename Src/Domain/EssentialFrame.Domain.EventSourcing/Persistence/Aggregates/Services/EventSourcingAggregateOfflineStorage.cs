using System.IO.Abstractions;
using EssentialFrame.Domain.Events;
using EssentialFrame.Domain.EventSourcing.Exceptions;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.ValueObjects.Core;
using EssentialFrame.Files;
using EssentialFrame.Serialization.Interfaces;
using EssentialFrame.Time;
using Microsoft.Extensions.Logging;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services;

internal sealed class EventSourcingAggregateOfflineStorage<TAggregateIdentifier> : IEventSourcingAggregateOfflineStorage
    where TAggregateIdentifier : TypedGuidIdentifier
{
    private const string EventsFileName = "events.json";
    private const string MetadataFileName = "metadata.txt";
    private const string IndexFileName = "boxes.csv";

    private readonly IDomainEventMapper<TAggregateIdentifier> _domainEventMapper;
    private readonly IFileStorage _fileStorage;
    private readonly IFileSystem _fileSystem;
    private readonly ILogger<EventSourcingAggregateOfflineStorage<TAggregateIdentifier>> _logger;
    private readonly string _offlineStorageDirectory;
    private readonly ISerializer _serializer;

    public EventSourcingAggregateOfflineStorage(IFileStorage fileStorage, IFileSystem fileSystem,
        ILogger<EventSourcingAggregateOfflineStorage<TAggregateIdentifier>> logger,
        IDomainEventMapper<TAggregateIdentifier> domainEventMapper, ISerializer serializer,
        string offlineStorageDirectory = null)
    {
        _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _domainEventMapper = domainEventMapper ?? throw new ArgumentNullException(nameof(domainEventMapper));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

        offlineStorageDirectory ??= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "EssentialFrame", "OfflineStorage", "Aggregates");

        _offlineStorageDirectory = offlineStorageDirectory;
    }

    public void Save(EventSourcingAggregateDataModel eventSourcingAggregate,
        IReadOnlyCollection<DomainEventDataModel> events)
    {
        try
        {
            string aggregateDirectory = _fileSystem.Path.Combine(_offlineStorageDirectory,
                eventSourcingAggregate.AggregateIdentifier.ToString());

            (string eventsContents, string metaDataContents) = CreateFileContents(eventSourcingAggregate, events);

            IFileInfo eventsFileInfo = _fileStorage.Create(aggregateDirectory, EventsFileName, eventsContents);
            _fileStorage.Create(aggregateDirectory, MetadataFileName, metaDataContents);

            string indexFileContents =
                $"{SystemClock.Now:yyyy/MM/dd-HH:mm},{eventSourcingAggregate},{eventSourcingAggregate.GetType().FullName},{eventsFileInfo.Length / 1024} KB,{eventSourcingAggregate.TenantIdentifier}\n";

            _fileStorage.Create(aggregateDirectory, IndexFileName, indexFileContents);
        }
        catch (Exception exception)
        {
            AggregateBoxingFailedException aggregateBoxingException = new(eventSourcingAggregate.AggregateIdentifier,
                eventSourcingAggregate.GetType(), exception);

            _logger.LogError(aggregateBoxingException,
                "Failed to save aggregate with id: {AggregateIdentifier} to offline storage",
                eventSourcingAggregate.AggregateIdentifier);

            throw aggregateBoxingException;
        }
    }

    public async Task SaveAsync(EventSourcingAggregateDataModel eventSourcingAggregate,
        IReadOnlyCollection<DomainEventDataModel> events, CancellationToken cancellationToken = default)
    {
        try
        {
            string aggregateDirectory = _fileSystem.Path.Combine(_offlineStorageDirectory,
                eventSourcingAggregate.AggregateIdentifier.ToString());

            (string eventsContents, string metaDataContents) = CreateFileContents(eventSourcingAggregate, events);

            IFileInfo eventsFileInfo = await _fileStorage.CreateAsync(aggregateDirectory, EventsFileName,
                eventsContents, cancellationToken: cancellationToken);

            await _fileStorage.CreateAsync(aggregateDirectory, MetadataFileName, metaDataContents,
                cancellationToken: cancellationToken);

            string indexFileContents =
                $"{SystemClock.Now:yyyy/MM/dd-HH:mm},{eventSourcingAggregate},{eventSourcingAggregate.GetType().FullName},{eventsFileInfo.Length / 1024} KB,{eventSourcingAggregate.TenantIdentifier}\n";

            await _fileStorage.CreateAsync(aggregateDirectory, IndexFileName, indexFileContents,
                cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            AggregateBoxingFailedException aggregateBoxingException = new(eventSourcingAggregate.AggregateIdentifier,
                eventSourcingAggregate.GetType(), exception);

            _logger.LogError(aggregateBoxingException,
                "Failed to save aggregate {AggregateIdentifier} to offline storage",
                eventSourcingAggregate.AggregateIdentifier);

            throw aggregateBoxingException;
        }
    }

    private (string serializedEvents, string metadata) CreateFileContents(
        EventSourcingAggregateDataModel eventSourcingAggregate, IReadOnlyCollection<DomainEventDataModel> events)
    {
        IReadOnlyCollection<IDomainEvent<TAggregateIdentifier>> domainEvents = _domainEventMapper.Map(events);
        string serializedEvents = _serializer.Serialize(domainEvents);

        Dictionary<string, string> metaData = new()
        {
            { "AggregateIdentifier", eventSourcingAggregate.AggregateIdentifier.ToString() },
            { "AggregateType", eventSourcingAggregate.GetType().FullName },
            { "AggregateVersion", eventSourcingAggregate.AggregateVersion.ToString() },
            { "Serialized Events", $"{events.Count:n0}" },
            { "Local Date/Time Boxed", $"{SystemClock.Now:dddd, MMMM d, yyyy HH:mm} Local" },
            { "Utc Date/Time Boxed", $"{SystemClock.UtcNow:dddd, MMMM d, yyyy HH:mm}" }
        };

        string metaDataContents = string.Join(Environment.NewLine, metaData.Select(kvp => $"{kvp.Key}: {kvp.Value}"));

        return (serializedEvents, metaDataContents);
    }
}