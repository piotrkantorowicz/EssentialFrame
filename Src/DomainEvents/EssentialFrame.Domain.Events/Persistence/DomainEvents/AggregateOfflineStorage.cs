using System.IO.Abstractions;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Exceptions;
using EssentialFrame.Domain.Events.Persistence.DomainEvents.Interfaces;
using EssentialFrame.Files;
using EssentialFrame.Serialization.Interfaces;
using EssentialFrame.Time;
using Microsoft.Extensions.Logging;

namespace EssentialFrame.Domain.Events.Persistence.DomainEvents;

internal sealed class AggregateOfflineStorage : IAggregateOfflineStorage
{
    private const string EventsFileName = "events.json";
    private const string MetadataFileName = "metadata.txt";
    private const string IndexFileName = "boxes.csv";
    private readonly IFileStorage _fileStorage;

    private readonly IFileSystem _fileSystem;
    private readonly ILogger<AggregateOfflineStorage> _logger;
    private readonly string _offlineStorageDirectory;
    private readonly ISerializer _serializer;

    public AggregateOfflineStorage(IFileStorage fileStorage, ISerializer serializer, IFileSystem fileSystem,
        ILogger<AggregateOfflineStorage> logger, string offlineStorageDirectory = null)
    {
        _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        offlineStorageDirectory ??= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "EssentialFrame", "OfflineStorage", "Aggregates");

        _offlineStorageDirectory = offlineStorageDirectory;
    }

    public void Save(AggregateRoot aggregate, IReadOnlyCollection<IDomainEvent> events)
    {
        try
        {
            string aggregateDirectory =
                _fileSystem.Path.Combine(_offlineStorageDirectory, aggregate.AggregateIdentifier.ToString());

            (string eventsContents, string metaDataContents) = CreateFileContents(aggregate, events);

            IFileInfo eventsFileInfo = _fileStorage.Create(aggregateDirectory, EventsFileName, eventsContents);
            _fileStorage.Create(aggregateDirectory, MetadataFileName, metaDataContents);

            string indexFileContents =
                $"{SystemClock.Now:yyyy/MM/dd-HH:mm},{aggregate},{aggregate.GetType().FullName},{eventsFileInfo.Length / 1024} KB,{aggregate.GetIdentity().Tenant.Name}\n";

            _fileStorage.Create(aggregateDirectory, IndexFileName, indexFileContents);
        }
        catch (Exception exception)
        {
            AggregateBoxingFailedException aggregateBoxingException =
                new(aggregate.AggregateIdentifier, aggregate.GetType(), exception);

            _logger.LogError(aggregateBoxingException,
                "Failed to save aggregate with id: {AggregateIdentifier} to offline storage",
                aggregate.AggregateIdentifier);

            throw aggregateBoxingException;
        }
    }

    public async Task SaveAsync(AggregateRoot aggregate, IReadOnlyCollection<IDomainEvent> events,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string aggregateDirectory =
                _fileSystem.Path.Combine(_offlineStorageDirectory, aggregate.AggregateIdentifier.ToString());

            (string eventsContents, string metaDataContents) = CreateFileContents(aggregate, events);

            IFileInfo eventsFileInfo = await _fileStorage.CreateAsync(aggregateDirectory, EventsFileName,
                eventsContents, cancellationToken: cancellationToken);

            await _fileStorage.CreateAsync(aggregateDirectory, MetadataFileName, metaDataContents,
                cancellationToken: cancellationToken);

            string indexFileContents =
                $"{SystemClock.Now:yyyy/MM/dd-HH:mm},{aggregate},{aggregate.GetType().FullName},{eventsFileInfo.Length / 1024} KB,{aggregate.GetIdentity().Tenant.Name}\n";

            await _fileStorage.CreateAsync(aggregateDirectory, IndexFileName, indexFileContents,
                cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            AggregateBoxingFailedException aggregateBoxingException =
                new(aggregate.AggregateIdentifier, aggregate.GetType(), exception);

            _logger.LogError(aggregateBoxingException,
                "Failed to save aggregate {AggregateIdentifier} to offline storage", aggregate.AggregateIdentifier);

            throw aggregateBoxingException;
        }
    }

    private (string eventsContents, string metadataContents) CreateFileContents(AggregateRoot aggregate,
        IReadOnlyCollection<IDomainEvent> events)
    {
        string eventsContents = _serializer.Serialize(events);

        Dictionary<string, string> metaData = new()
        {
            { "AggregateIdentifier", aggregate.AggregateIdentifier.ToString() },
            { "AggregateType", aggregate.GetType().FullName },
            { "AggregateVersion", aggregate.AggregateVersion.ToString() },
            { "Serialized Events", $"{events.Count:n0}" },
            { "Local Date/Time Boxed", $"{SystemClock.Now:dddd, MMMM d, yyyy HH:mm} Local" },
            { "Utc Date/Time Boxed", $"{SystemClock.UtcNow:dddd, MMMM d, yyyy HH:mm}" }
        };

        string metaDataContents = string.Join(Environment.NewLine, metaData.Select(kvp => $"{kvp.Key}: {kvp.Value}"));

        return (eventsContents, metaDataContents);
    }
}