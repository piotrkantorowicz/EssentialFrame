using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Persistence.Models;
using EssentialFrame.Domain.Persistence.Services.Interfaces;

namespace EssentialFrame.Domain.Persistence.Services;

internal sealed class DefaultAggregateStore : IAggregateStore
{
    private readonly ICache<Guid, AggregateDataModel> _aggregateCache;
    private readonly IAggregateOfflineStorage _aggregateOfflineStorage;

    public DefaultAggregateStore(ICache<Guid, AggregateDataModel> aggregateCache,
        IAggregateOfflineStorage aggregateOfflineStorage)
    {
        _aggregateCache = aggregateCache ?? throw new ArgumentNullException(nameof(aggregateCache));

        _aggregateOfflineStorage =
            aggregateOfflineStorage ?? throw new ArgumentNullException(nameof(aggregateOfflineStorage));
    }

    public bool Exists(Guid aggregateIdentifier)
    {
        return _aggregateCache.Exists(aggregateIdentifier);
    }

    public async Task<bool> ExistsAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_aggregateCache.Exists(aggregateIdentifier));
    }

    public AggregateDataModel Get(Guid aggregateIdentifier)
    {
        return _aggregateCache.Get(aggregateIdentifier);
    }

    public async Task<AggregateDataModel> GetAsync(Guid aggregateIdentifier,
        CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(Get(aggregateIdentifier));
    }

    public IEnumerable<Guid> GetExpired()
    {
        return _aggregateCache.GetMany((_, v) => v.IsDeleted)?.Select(v => v.AggregateIdentifier);
    }

    public async Task<IEnumerable<Guid>> GetExpiredAsync(CancellationToken cancellationToken = default)
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

    public void Box(Guid aggregateIdentifier)
    {
        AggregateDataModel aggregate = _aggregateCache.Get(aggregateIdentifier);

        _aggregateOfflineStorage.Save(aggregate);
    }

    public async Task BoxAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default)
    {
        AggregateDataModel aggregate = _aggregateCache.Get(aggregateIdentifier);

        await _aggregateOfflineStorage.SaveAsync(aggregate, cancellationToken);
    }
}