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

    public async Task<bool> ExistsAsync(string aggregateIdentifier, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_aggregateCache.Exists(aggregateIdentifier));
    }

    public AggregateDataModel Get(string aggregateIdentifier)
    {
        return _aggregateCache.Get(aggregateIdentifier);
    }

    public async Task<AggregateDataModel> GetAsync(string aggregateIdentifier,
        CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(Get(aggregateIdentifier));
    }

    public IEnumerable<string> GetExpired()
    {
        return _aggregateCache.GetMany((_, v) => v.IsDeleted)?.Select(v => v.AggregateIdentifier);
    }

    public async Task<IEnumerable<string>> GetExpiredAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(GetExpired());
    }

    public void Save(AggregateDataModel aggregate)
    {
        _aggregateCache.Add(aggregate.AggregateIdentifier, aggregate);
    }

    public async Task SaveAsync(AggregateDataModel aggregate, CancellationToken cancellationToken = default)
    {
        Save(aggregate);

        await Task.CompletedTask;
    }

    public void Box(string aggregateIdentifier)
    {
        AggregateDataModel aggregate = _aggregateCache.Get(aggregateIdentifier);

        _aggregateOfflineStorage.Save(aggregate);
    }

    public async Task BoxAsync(string aggregateIdentifier, CancellationToken cancellationToken = default)
    {
        AggregateDataModel aggregate = _aggregateCache.Get(aggregateIdentifier);

        await _aggregateOfflineStorage.SaveAsync(aggregate, cancellationToken);
    }
}