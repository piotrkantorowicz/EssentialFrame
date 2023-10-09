using System.IO.Abstractions;
using System.Text;
using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Persistence.Mappers.Interfaces;
using EssentialFrame.Domain.Persistence.Models;
using EssentialFrame.Files.Interfaces;
using EssentialFrame.Serialization.Interfaces;
using EssentialFrame.Time;
using Microsoft.Extensions.Logging;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services;

internal sealed class
    EventSourcingAggregateOfflineStorage<TAggregateIdentifier, TType> : IEventSourcingAggregateOfflineStorage
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    private const string EventsFileName = "events.json";
    private const string MetadataFileName = "metadata.txt";
    private const string IndexFileName = "boxes.csv";

    private readonly IDomainEventMapper<TAggregateIdentifier, TType> _domainEventMapper;
    private readonly IFileStorage _fileStorage;
    private readonly IFileSystem _fileSystem;
    private readonly ILogger<EventSourcingAggregateOfflineStorage<TAggregateIdentifier, TType>> _logger;
    private readonly string _offlineStorageDirectory;
    private readonly ISerializer _serializer;

    public EventSourcingAggregateOfflineStorage(IFileStorage fileStorage, IFileSystem fileSystem,
        ILogger<EventSourcingAggregateOfflineStorage<TAggregateIdentifier, TType>> logger,
        IDomainEventMapper<TAggregateIdentifier, TType> domainEventMapper, ISerializer serializer,
        string offlineStorageDirectory)
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
        IReadOnlyCollection<DomainEventDataModel> events, Encoding encoding)
    {
        try
        {
            string aggregateDirectory = _fileSystem.Path.Combine(_offlineStorageDirectory,
                eventSourcingAggregate.AggregateIdentifier);

            (string eventsContent, string metaDataContent) = CreateFileContents(eventSourcingAggregate, events);

            IFileInfo eventsFileInfo = _fileStorage.Create(aggregateDirectory, EventsFileName, eventsContent, encoding);
            _fileStorage.Create(aggregateDirectory, MetadataFileName, metaDataContent, encoding);

            string indexFileContent =
                $"{SystemClock.Now:yyyy/MM/dd-HH:mm},{eventSourcingAggregate},{eventSourcingAggregate.GetType().FullName},{eventsFileInfo.Length / 1024} KB,{eventSourcingAggregate.TenantIdentifier}\n";

            _fileStorage.Create(aggregateDirectory, IndexFileName, indexFileContent, encoding);
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
        IReadOnlyCollection<DomainEventDataModel> events, Encoding encoding, CancellationToken cancellationToken)
    {
        try
        {
            string aggregateDirectory = _fileSystem.Path.Combine(_offlineStorageDirectory,
                eventSourcingAggregate.AggregateIdentifier);

            (string eventsContent, string metaDataContent) = CreateFileContents(eventSourcingAggregate, events);

            IFileInfo eventsFileInfo = await _fileStorage.CreateAsync(aggregateDirectory, EventsFileName, eventsContent,
                encoding,
                cancellationToken: cancellationToken);

            await _fileStorage.CreateAsync(aggregateDirectory, MetadataFileName, metaDataContent, encoding,
                cancellationToken);

            string indexFileContents =
                $"{SystemClock.Now:yyyy/MM/dd-HH:mm},{eventSourcingAggregate},{eventSourcingAggregate.GetType().FullName},{eventsFileInfo.Length / 1024} KB,{eventSourcingAggregate.TenantIdentifier}\n";

            await _fileStorage.CreateAsync(aggregateDirectory, IndexFileName, indexFileContents, encoding,
                cancellationToken);
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
        IReadOnlyCollection<IDomainEvent<TAggregateIdentifier, TType>> domainEvents = _domainEventMapper.Map(events);
        string serializedEvents = _serializer.Serialize(domainEvents);

        Dictionary<string, string> metaData = new()
        {
            { "AggregateIdentifier", eventSourcingAggregate.AggregateIdentifier },
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