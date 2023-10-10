using System.IO.Abstractions;
using System.Text;
using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.Exceptions;
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
    private const int MetaDataPropertiesLength = 7;

    private readonly IFileStorage _fileStorage;
    private readonly IFileSystem _fileSystem;
    private readonly ILogger<EventSourcingAggregateOfflineStorage<TAggregateIdentifier, TType>> _logger;
    private readonly string _offlineStorageDirectory;
    private readonly ISerializer _serializer;

    public EventSourcingAggregateOfflineStorage(IFileStorage fileStorage, IFileSystem fileSystem,
        ILogger<EventSourcingAggregateOfflineStorage<TAggregateIdentifier, TType>> logger, ISerializer serializer,
        string offlineStorageDirectory)
    {
        _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

        offlineStorageDirectory ??= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "EssentialFrame", "OfflineStorage", "Aggregates");

        _offlineStorageDirectory = offlineStorageDirectory;
    }

    public EventSourcingAggregateWithEventsModel Get(string aggregateIdentifier, Encoding encoding)
    {
        try
        {
            string metaData = _fileStorage.Read(_fileSystem.Path.Combine(_offlineStorageDirectory, aggregateIdentifier),
                MetadataFileName, encoding);

            if (metaData is null)
            {
                throw new AggregateUnboxingFailedException(aggregateIdentifier);
            }

            Dictionary<string, string> metaDataDictionary =
                _serializer.Deserialize<Dictionary<string, string>>(metaData, typeof(Dictionary<string, string>));

            if (metaDataDictionary.Count < MetaDataPropertiesLength)
            {
                throw new AggregateUnboxingFailedException(aggregateIdentifier);
            }

            string domainEvents =
                _fileStorage.Read(_fileSystem.Path.Combine(_offlineStorageDirectory, aggregateIdentifier),
                    EventsFileName, encoding);

            if (domainEvents is null)
            {
                throw new AggregateUnboxingFailedException(aggregateIdentifier);
            }

            return new EventSourcingAggregateWithEventsModel(
                new EventSourcingAggregateDataModel
                {
                    AggregateIdentifier = metaDataDictionary["AggregateIdentifier"],
                    AggregateVersion =
                        int.TryParse(metaDataDictionary["AggregateVersion"], out int aggregateVersion)
                            ? aggregateVersion
                            : 0,
                    TenantIdentifier =
                        Guid.TryParse(metaDataDictionary["TenantIdentifier"], out Guid tenantIdentifier)
                            ? tenantIdentifier
                            : Guid.Empty
                },
                _serializer.Deserialize<IReadOnlyCollection<DomainEventDataModel>>(domainEvents,
                    typeof(IReadOnlyCollection<IDomainEvent<TAggregateIdentifier, TType>>)));
        }
        catch (Exception exception)
        {
            AggregateUnboxingFailedException aggregateBoxingException = new(aggregateIdentifier, exception);

            _logger.LogError(aggregateBoxingException,
                "Failed to save aggregate with id: {AggregateIdentifier} to offline storage", aggregateIdentifier);

            throw aggregateBoxingException;
        }
    }

    public async Task<EventSourcingAggregateWithEventsModel> GetAsync(string aggregateIdentifier, Encoding encoding,
        CancellationToken cancellationToken)
    {
        try
        {
            string metaData = await _fileStorage.ReadAsync(
                _fileSystem.Path.Combine(_offlineStorageDirectory, aggregateIdentifier), MetadataFileName, encoding,
                cancellationToken);

            if (metaData is null)
            {
                throw new AggregateUnboxingFailedException(aggregateIdentifier);
            }

            Dictionary<string, string> metaDataDictionary =
                _serializer.Deserialize<Dictionary<string, string>>(metaData, typeof(Dictionary<string, string>));

            if (metaDataDictionary.Count < MetaDataPropertiesLength)
            {
                throw new AggregateUnboxingFailedException(aggregateIdentifier);
            }

            string domainEvents = await _fileStorage.ReadAsync(
                _fileSystem.Path.Combine(_offlineStorageDirectory, aggregateIdentifier), EventsFileName, encoding,
                cancellationToken);

            if (domainEvents is null)
            {
                throw new AggregateUnboxingFailedException(aggregateIdentifier);
            }

            return new EventSourcingAggregateWithEventsModel(
                new EventSourcingAggregateDataModel
                {
                    AggregateIdentifier = metaDataDictionary["AggregateIdentifier"],
                    AggregateVersion =
                        int.TryParse(metaDataDictionary["AggregateVersion"], out int aggregateVersion)
                            ? aggregateVersion
                            : 0,
                    TenantIdentifier =
                        Guid.TryParse(metaDataDictionary["TenantIdentifier"], out Guid tenantIdentifier)
                            ? tenantIdentifier
                            : Guid.Empty
                },
                _serializer.Deserialize<IReadOnlyCollection<DomainEventDataModel>>(domainEvents,
                    typeof(IReadOnlyCollection<IDomainEvent<TAggregateIdentifier, TType>>)));
        }
        catch (Exception exception)
        {
            AggregateUnboxingFailedException aggregateBoxingException = new(aggregateIdentifier, exception);

            _logger.LogError(aggregateBoxingException,
                "Failed to save aggregate with id: {AggregateIdentifier} to offline storage", aggregateIdentifier);

            throw aggregateBoxingException;
        }
    }

    public void Save(EventSourcingAggregateDataModel eventSourcingAggregate,
        IReadOnlyCollection<DomainEventDataModel> events, Encoding encoding)
    {
        try
        {
            string aggregateDirectory = _fileSystem.Path.Combine(_offlineStorageDirectory,
                eventSourcingAggregate.AggregateIdentifier);

            IFileInfo eventsFileInfo = _fileStorage.Create(aggregateDirectory, EventsFileName,
                _serializer.Serialize(events), encoding);

            _fileStorage.Create(aggregateDirectory, MetadataFileName, CreateMetaData(eventSourcingAggregate, events),
                encoding);

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

            IFileInfo eventsFileInfo = await _fileStorage.CreateAsync(aggregateDirectory, EventsFileName,
                _serializer.Serialize(events), encoding, cancellationToken);

            await _fileStorage.CreateAsync(aggregateDirectory, MetadataFileName,
                CreateMetaData(eventSourcingAggregate, events), encoding, cancellationToken);

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

    private string CreateMetaData(EventSourcingAggregateDataModel eventSourcingAggregate,
        IReadOnlyCollection<DomainEventDataModel> events)
    {
        Dictionary<string, string> metaData = new()
        {
            { "AggregateIdentifier", eventSourcingAggregate.AggregateIdentifier },
            { "AggregateType", eventSourcingAggregate.GetType().FullName },
            { "AggregateVersion", eventSourcingAggregate.AggregateVersion.ToString() },
            { "TenantIdentifier", eventSourcingAggregate.TenantIdentifier.ToString() },
            { "Serialized Events", $"{events.Count:n0}" },
            { "Local Date/Time Boxed", $"{SystemClock.Now:dddd, MMMM d, yyyy HH:mm} Local" },
            { "Utc Date/Time Boxed", $"{SystemClock.UtcNow:dddd, MMMM d, yyyy HH:mm}" }
        };

        return _serializer.Serialize(metaData);
    }
}