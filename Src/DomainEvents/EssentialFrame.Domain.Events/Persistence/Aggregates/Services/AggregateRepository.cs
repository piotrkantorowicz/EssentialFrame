using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Exceptions;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.Factories;
using EssentialFrame.Identity;

namespace EssentialFrame.Domain.Events.Persistence.Aggregates.Services;

public sealed class AggregateRepository : IAggregateRepository
{
    private readonly IAggregateMapper _aggregateMapper;
    private readonly IDomainEventMapper _domainEventMapper;
    private readonly IIdentityService _identityService;
    private readonly IAggregateStore _store;

    public AggregateRepository(IAggregateStore store, IDomainEventMapper domainEventMapper,
        IAggregateMapper aggregateMapper, IIdentityService identityService)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _domainEventMapper = domainEventMapper ?? throw new ArgumentNullException(nameof(domainEventMapper));
        _aggregateMapper = aggregateMapper ?? throw new ArgumentNullException(nameof(aggregateMapper));
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
    }

    public T Get<T>(Guid aggregate) where T : AggregateRoot
    {
        return Rehydrate<T>(aggregate);
    }

    public Task<T> GetAsync<T>(Guid aggregate, CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        return RehydrateAsync<T>(aggregate, cancellationToken);
    }

    public IDomainEvent[] Save<T>(T aggregate, int? version) where T : AggregateRoot
    {
        if (version != null && _store.Exists(aggregate.AggregateIdentifier, version.Value))
        {
            throw new ConcurrencyException(aggregate.AggregateIdentifier);
        }

        AggregateDataModel aggregateDataModel = _aggregateMapper.Map(aggregate);
        IDomainEvent[] domainEvents = aggregate.FlushUncommittedChanges();
        IEnumerable<DomainEventDataModel> domainEventDataModels = domainEvents.Select(e => _domainEventMapper.Map(e));

        _store.Save(aggregateDataModel, domainEventDataModels);

        return domainEvents;
    }

    public async Task<IDomainEvent[]> SaveAsync<T>(T aggregate, int? version = null,
        CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        if (version != null &&
            await _store.ExistsAsync(aggregate.AggregateIdentifier, version.Value, cancellationToken))
        {
            throw new ConcurrencyException(aggregate.AggregateIdentifier);
        }

        AggregateDataModel aggregateDataModel = _aggregateMapper.Map(aggregate);
        IDomainEvent[] domainEvents = aggregate.FlushUncommittedChanges();
        IEnumerable<DomainEventDataModel> domainEventDataModels = domainEvents.Select(e => _domainEventMapper.Map(e));

        await _store.SaveAsync(aggregateDataModel, domainEventDataModels, cancellationToken);

        return domainEvents;
    }

    private T Rehydrate<T>(Guid id) where T : AggregateRoot
    {
        IReadOnlyCollection<DomainEventDataModel> events = _store.Get(id, -1);
        T aggregate = RehydrateInternal<T>(id, events);

        return aggregate;
    }

    private async Task<T> RehydrateAsync<T>(Guid id, CancellationToken cancellationToken = default)
        where T : AggregateRoot
    {
        IReadOnlyCollection<DomainEventDataModel> events = await _store.GetAsync(id, -1, cancellationToken);

        T aggregate = RehydrateInternal<T>(id, events);

        return aggregate;
    }

    private T RehydrateInternal<T>(Guid id, IEnumerable<DomainEventDataModel> eventsData) where T : AggregateRoot
    {
        List<IDomainEvent> events = eventsData.Select(e => _domainEventMapper.Map(e)).ToList();

        if (!events.Any())
        {
            throw new AggregateNotFoundException(typeof(T), id);
        }

        AggregateDataModel aggregateDataModel = _store.Get(id);

        if (aggregateDataModel.IsDeleted)
        {
            throw new AggregateDeletedException(id, typeof(T));
        }

        T aggregate = GenericAggregateFactory<T>.CreateAggregate(id, 0, _identityService);
        aggregate.Rehydrate(events);

        return aggregate;
    }
}