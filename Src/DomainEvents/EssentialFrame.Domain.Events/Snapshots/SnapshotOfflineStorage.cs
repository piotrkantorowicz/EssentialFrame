using System.IO.Abstractions;
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

    public void Save(Snapshot snapshot)
    {
        try
        {
            string aggregateDirectory =
                _fileSystem.Path.Combine(_offlineStorageDirectory, snapshot.AggregateIdentifier.ToString());

            string aggregateState = GetSerializedState(snapshot);

            _fileStorage.Create(aggregateDirectory, SnapshotFilename, aggregateState);
        }
        catch (Exception exception)
        {
            SnapshotBoxingFailedException aggregateBoxingException =
                SnapshotBoxingFailedException.Unexpected(snapshot.AggregateIdentifier, exception);

            _logger.LogError(aggregateBoxingException,
                "Failed to save aggregate {AggregateIdentifier} to offline storage", snapshot.AggregateIdentifier);

            throw aggregateBoxingException;
        }
    }

    public async Task SaveAsync(Snapshot snapshot, CancellationToken cancellationToken = default)
    {
        try
        {
            string aggregateDirectory =
                _fileSystem.Path.Combine(_offlineStorageDirectory, snapshot.AggregateIdentifier.ToString());

            string aggregateState = GetSerializedState(snapshot);

            await _fileStorage.CreateAsync(aggregateDirectory, SnapshotFilename, aggregateState,
                cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            SnapshotBoxingFailedException aggregateBoxingException =
                SnapshotBoxingFailedException.Unexpected(snapshot.AggregateIdentifier, exception);

            _logger.LogError(aggregateBoxingException,
                "Failed to save aggregate {AggregateIdentifier} to offline storage", snapshot.AggregateIdentifier);

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

            if (aggregateState is null)
            {
                throw SnapshotUnboxingFailedException.SnapshotNotFound(aggregateIdentifier);
            }

            return new Snapshot(aggregateIdentifier, 1, aggregateState);
        }
        catch (Exception exception)
        {
            SnapshotUnboxingFailedException aggregateBoxingException =
                SnapshotUnboxingFailedException.Unexpected(aggregateIdentifier, exception);

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

            if (aggregateState is null)
            {
                throw SnapshotUnboxingFailedException.SnapshotNotFound(aggregateIdentifier);
            }

            return new Snapshot(aggregateIdentifier, 1, aggregateState);
        }
        catch (Exception exception)
        {
            SnapshotUnboxingFailedException aggregateBoxingException =
                SnapshotUnboxingFailedException.Unexpected(aggregateIdentifier, exception);

            _logger.LogError(aggregateBoxingException,
                "Failed to save aggregate {AggregateIdentifier} to offline storage", aggregateIdentifier);

            throw aggregateBoxingException;
        }
    }

    private string GetSerializedState(Snapshot snapshot)
    {
        string serializedState;

        if (snapshot.AggregateState is string alreadySerializedState)
        {
            serializedState = alreadySerializedState;
        }
        else
        {
            serializedState = _serializer.Serialize(snapshot.AggregateState);
        }

        return serializedState;
    }
}