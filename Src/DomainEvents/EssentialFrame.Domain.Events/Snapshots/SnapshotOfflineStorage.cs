using System.IO.Abstractions;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Exceptions;
using EssentialFrame.Domain.Events.Snapshots.Interfaces;
using EssentialFrame.Domain.Snapshots;
using EssentialFrame.Files;
using EssentialFrame.Serialization.Interfaces;
using Microsoft.Extensions.Logging;

namespace EssentialFrame.Domain.Events.Snapshots;

internal sealed class SnapshotOfflineStorage : ISnapshotOfflineStorage
{
    private const string SnapshotFilename = "Snapshot.json";

    private readonly IFileStorage _fileStorage;
    private readonly ISerializer _serializer;
    private readonly IFileSystem _fileSystem;
    private readonly ILogger<SnapshotOfflineStorage> _logger;
    private readonly string _offlineStorageDirectory;

    public SnapshotOfflineStorage(IFileStorage fileStorage, ISerializer serializer, IFileSystem fileSystem,
        ILogger<SnapshotOfflineStorage> logger, string offlineStorageDirectory = null)
    {
        _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        offlineStorageDirectory ??= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "EssentialFrame", "OfflineStorage", "Snapshots");

        _offlineStorageDirectory = offlineStorageDirectory;
    }

    public void Save(AggregateRoot aggregate)
    {
        try
        {
            string aggregateDirectory =
                _fileSystem.Path.Combine(_offlineStorageDirectory, aggregate.AggregateIdentifier.ToString());

            string fileContents = _serializer.Serialize(aggregate.State);

            _fileStorage.Create(aggregateDirectory, SnapshotFilename, fileContents);
        }
        catch (Exception exception)
        {
            AggregateBoxingFailedException aggregateBoxingException =
                new AggregateBoxingFailedException(aggregate.AggregateIdentifier, aggregate.GetType(), exception);

            _logger.LogError(aggregateBoxingException,
                "Failed to save aggregate {AggregateIdentifier} to offline storage", aggregate.AggregateIdentifier);

            throw aggregateBoxingException;
        }
    }

    public async Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken = default)
    {
        try
        {
            string aggregateDirectory =
                _fileSystem.Path.Combine(_offlineStorageDirectory, aggregate.AggregateIdentifier.ToString());

            string fileContents = _serializer.Serialize(aggregate.State);

            await _fileStorage.CreateAsync(aggregateDirectory, SnapshotFilename, fileContents,
                cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            AggregateBoxingFailedException aggregateBoxingException =
                new AggregateBoxingFailedException(aggregate.AggregateIdentifier, aggregate.GetType(), exception);

            _logger.LogError(aggregateBoxingException,
                "Failed to save aggregate {AggregateIdentifier} to offline storage", aggregate.AggregateIdentifier);

            throw aggregateBoxingException;
        }
    }

    public Snapshot Restore(Guid aggregateIdentifier)
    {
        try
        {
            string aggregateState =
                _fileStorage.Read(_fileSystem.Path.Combine(_offlineStorageDirectory, aggregateIdentifier.ToString()),
                    SnapshotFilename);

            return new Snapshot(aggregateIdentifier, 1, aggregateState);
        }
        catch (Exception exception)
        {
            AggregateUnBoxingFailedException aggregateBoxingException =
                new AggregateUnBoxingFailedException(aggregateIdentifier, exception);

            _logger.LogError(aggregateBoxingException,
                "Failed to save aggregate {AggregateIdentifier} to offline storage", aggregateIdentifier);

            throw aggregateBoxingException;
        }
    }

    public async Task<Snapshot> RestoreAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default)
    {
        try
        {
            string aggregateState = await _fileStorage.ReadAsync(
                _fileSystem.Path.Combine(_offlineStorageDirectory, aggregateIdentifier.ToString()), SnapshotFilename,
                cancellationToken: cancellationToken);

            return new Snapshot(aggregateIdentifier, 1, aggregateState);
        }
        catch (Exception exception)
        {
            AggregateUnBoxingFailedException aggregateBoxingException =
                new AggregateUnBoxingFailedException(aggregateIdentifier, exception);

            _logger.LogError(aggregateBoxingException,
                "Failed to save aggregate {AggregateIdentifier} to offline storage", aggregateIdentifier);

            throw aggregateBoxingException;
        }
    }
}