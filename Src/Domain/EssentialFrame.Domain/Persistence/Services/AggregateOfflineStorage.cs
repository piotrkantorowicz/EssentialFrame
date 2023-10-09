using System.IO.Abstractions;
using System.Text;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Persistence.Models;
using EssentialFrame.Domain.Persistence.Services.Interfaces;
using EssentialFrame.Files.Interfaces;
using EssentialFrame.Serialization.Interfaces;
using EssentialFrame.Time;
using Microsoft.Extensions.Logging;

namespace EssentialFrame.Domain.Persistence.Services;

internal sealed class AggregateOfflineStorage : IAggregateOfflineStorage
{
    private const string StateFileName = "state.json";
    private const string MetadataFileName = "metadata.txt";
    private const string IndexFileName = "boxes.csv";

    private readonly IFileStorage _fileStorage;
    private readonly IFileSystem _fileSystem;
    private readonly ILogger<AggregateOfflineStorage> _logger;
    private readonly string _offlineStorageDirectory;
    private readonly ISerializer _serializer;

    public AggregateOfflineStorage(IFileStorage fileStorage, IFileSystem fileSystem,
        ILogger<AggregateOfflineStorage> logger, ISerializer serializer, string offlineStorageDirectory)
    {
        _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

        offlineStorageDirectory ??= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "EssentialFrame", "OfflineStorage", "Aggregates");

        _offlineStorageDirectory = offlineStorageDirectory;
    }

    public void Save(AggregateDataModel aggregate, Encoding encoding)
    {
        try
        {
            string aggregateDirectory =
                _fileSystem.Path.Combine(_offlineStorageDirectory, aggregate.AggregateIdentifier);

            (string stateContent, string metaDataContent) = CreateFileContents(aggregate);

            IFileInfo stateFileInfo = _fileStorage.Create(aggregateDirectory, StateFileName, stateContent, encoding);

            _fileStorage.Create(aggregateDirectory, MetadataFileName, metaDataContent, encoding);

            string indexFileContents =
                $"{SystemClock.Now:yyyy/MM/dd-HH:mm},{aggregate},{aggregate.GetType().FullName},{stateFileInfo.Length / 1024} KB,{aggregate.TenantIdentifier}\n";

            _fileStorage.Create(aggregateDirectory, IndexFileName, indexFileContents, encoding);
        }
        catch (Exception exception)
        {
            AggregateBoxingFailedException aggregateBoxingException = new(aggregate.AggregateIdentifier,
                aggregate.GetType(), exception);

            _logger.LogError(aggregateBoxingException,
                "Failed to save aggregate with id: {AggregateIdentifier} to offline storage",
                aggregate.AggregateIdentifier);

            throw aggregateBoxingException;
        }
    }

    public async Task SaveAsync(AggregateDataModel aggregate, Encoding encoding, CancellationToken cancellationToken)
    {
        try
        {
            string aggregateDirectory = _fileSystem.Path.Combine(_offlineStorageDirectory,
                aggregate.AggregateIdentifier);

            (string stateContent, string metaDataContent) = CreateFileContents(aggregate);

            IFileInfo stateFileInfo = await _fileStorage.CreateAsync(aggregateDirectory, StateFileName, stateContent,
                encoding,
                cancellationToken: cancellationToken);

            await _fileStorage.CreateAsync(aggregateDirectory, MetadataFileName, metaDataContent, encoding,
                cancellationToken: cancellationToken);

            string indexFileContents =
                $"{SystemClock.Now:yyyy/MM/dd-HH:mm},{aggregate},{aggregate.GetType().FullName},{stateFileInfo.Length / 1024} KB,{aggregate.TenantIdentifier}\n";

            await _fileStorage.CreateAsync(aggregateDirectory, IndexFileName, indexFileContents, encoding,
                cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            AggregateBoxingFailedException aggregateBoxingException = new(aggregate.AggregateIdentifier,
                aggregate.GetType(), exception);

            _logger.LogError(aggregateBoxingException,
                "Failed to save aggregate {AggregateIdentifier} to offline storage", aggregate.AggregateIdentifier);

            throw aggregateBoxingException;
        }
    }

    private (string state, string metadata) CreateFileContents(AggregateDataModel aggregate)
    {
        string aggregateState = _serializer.Serialize(aggregate.State);

        Dictionary<string, string> metaData = new()
        {
            { "AggregateIdentifier", aggregate.AggregateIdentifier },
            { "AggregateType", aggregate.GetType().FullName },
            { "State", _serializer.Serialize(aggregateState) },
            { "Local Date/Time Boxed", $"{SystemClock.Now:dddd, MMMM d, yyyy HH:mm} Local" },
            { "Utc Date/Time Boxed", $"{SystemClock.UtcNow:dddd, MMMM d, yyyy HH:mm}" }
        };

        string metaDataContent = string.Join(Environment.NewLine, metaData.Select(kvp => $"{kvp.Key}: {kvp.Value}"));

        return (aggregateState, metaDataContent);
    }
}