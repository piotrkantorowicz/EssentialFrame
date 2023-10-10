using System.Text;
using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Persistence.Models;
using EssentialFrame.Domain.Persistence.Services.Interfaces;

namespace EssentialFrame.Domain.Persistence.Services;

internal sealed class DefaultAggregateStore : IAggregateStore
{
    private readonly ICache<string, AggregateDataModel> _aggregateCache;
    private readonly IAggregateOfflineStorage _aggregateOfflineStorage;

    public DefaultAggregateStore(ICache<string, AggregateDataModel> aggregateCache,
        IAggregateOfflineStorage aggregateOfflineStorage)
    {
        _aggregateCache = aggregateCache ?? throw new ArgumentNullException(nameof(aggregateCache));

        _aggregateOfflineStorage =
            aggregateOfflineStorage ?? throw new ArgumentNullException(nameof(aggregateOfflineStorage));
    }

    public bool Exists(string aggregateIdentifier)
    {
        return _aggregateCache.Exists(aggregateIdentifier);
    }

    public async Task<bool> ExistsAsync(string aggregateIdentifier, CancellationToken cancellationToken)
    {
        return await Task.FromResult(_aggregateCache.Exists(aggregateIdentifier));
    }

    public AggregateDataModel Get(string aggregateIdentifier)
    {
        return _aggregateCache.Get(aggregateIdentifier);
    }

    public async Task<AggregateDataModel> GetAsync(string aggregateIdentifier, CancellationToken cancellationToken)
    {
        return await Task.FromResult(Get(aggregateIdentifier));
    }

    public void Save(AggregateDataModel aggregate)
    {
        _aggregateCache.Add(aggregate.AggregateIdentifier, aggregate);
    }

    public async Task SaveAsync(AggregateDataModel aggregate, CancellationToken cancellationToken)
    {
        Save(aggregate);

        await Task.CompletedTask;
    }

    public void Box(string aggregateIdentifier, Encoding encoding)
    {
        AggregateDataModel aggregate = _aggregateCache.Get(aggregateIdentifier);

        _aggregateOfflineStorage.Save(aggregate, encoding);
    }

    public async Task BoxAsync(string aggregateIdentifier, Encoding encoding, CancellationToken cancellationToken)
    {
        AggregateDataModel aggregate = _aggregateCache.Get(aggregateIdentifier);

        await _aggregateOfflineStorage.SaveAsync(aggregate, encoding, cancellationToken);
    }

    public AggregateDataModel Unbox(string aggregateIdentifier, Encoding encoding)
    {
        return _aggregateOfflineStorage.Get(aggregateIdentifier, encoding);
    }

    public async Task<AggregateDataModel> UnboxAsync(string aggregateIdentifier, Encoding encoding,
        CancellationToken cancellationToken)
    {
        return await _aggregateOfflineStorage.GetAsync(aggregateIdentifier, encoding, cancellationToken);
    }
}
    