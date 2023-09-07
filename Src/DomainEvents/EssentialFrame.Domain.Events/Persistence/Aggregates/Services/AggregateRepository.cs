using EssentialFrame.Domain.Events.Core.Aggregates;
using EssentialFrame.Domain.Events.Core.Factories;
using EssentialFrame.Domain.Events.Exceptions;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Identity;

namespace EssentialFrame.Domain.Events.Persistence.Aggregates.Services;

public sealed class AggregateRepository : IAggregateRepository
{
    private readonly IAggregateMapper _aggregateMapper;
    private readonly IDomainEventMapper _domainEventMapper;
    private readonly IAggregateStore _aggregateStore;

    public AggregateRepository(IAggregateStore aggregateStore, IDomainEventMapper domainEventMapper,
        IAggregateMapper aggregateMapper)
    {
        _aggregateStore = aggregateStore ?? throw new ArgumentNullException(nameof(aggregateStore));
        _domainEventMapper = domainEventMapper ?? throw new ArgumentNullException(nameof(domainEventMapper));
        _aggregateMapper = aggregateMapper ?? throw new ArgumentNullException(nameof(aggregateMapper));
    }

    public T Get<T>(Guid aggregate, IIdentityContext identityContext) where T : AggregateRoot
    {
        return Rehydrate<T>(aggregate, identityContext);
    }

    public Task<T> GetAsync<T>(Guid aggregate, IIdentityContext identityContext,
        CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        return RehydrateAsync<T>(aggregate, identityContext, cancellationToken);
    }

    public IDomainEvent[] Save<T>(T aggregate, int? version) where T : AggregateRoot
    {
        if (version != null && _aggregateStore.Exists(aggregate.AggregateIdentifier, version.Value))
        {
            throw new ConcurrencyException(aggregate.AggregateIdentifier);
        }

        AggregateDataModel aggregateDataModel = _aggregateMapper.Map(aggregate);
        IDomainEvent[] domainEvents = aggregate.FlushUncommittedChanges();
        IEnumerable<DomainEventDataModel> domainEventDataModels = _domainEventMapper.Map(domainEvents);

        _aggregateStore.Save(aggregateDataModel, domainEventDataModels);

        return domainEvents;
    }

    public async Task<IDomainEvent[]> SaveAsync<T>(T aggregate, int? version = null,
        CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        if (version != null &&
            await _aggregateStore.ExistsAsync(aggregate.AggregateIdentifier, version.Value, cancellationToken))
        {
            throw new ConcurrencyException(aggregate.AggregateIdentifier);
        }

        AggregateDataModel aggregateDataModel = _aggregateMapper.Map(aggregate);
        IDomainEvent[] domainEvents = aggregate.FlushUncommittedChanges();
        IEnumerable<DomainEventDataModel> domainEventDataModels = _domainEventMapper.Map(domainEvents);

        await _aggregateStore.SaveAsync(aggregateDataModel, domainEventDataModels, cancellationToken);

        return domainEvents;
    }

    private T Rehydrate<T>(Guid id, IIdentityContext identityContext) where T : AggregateRoot
    {
        AggregateDataModel aggregateDataModel = _aggregateStore.Get(id);
        IReadOnlyCollection<DomainEventDataModel> eventsData = _aggregateStore.Get(id, -1);

        if (aggregateDataModel?.IsDeleted == true)
        {
            throw new AggregateDeletedException(aggregateDataModel.AggregateIdentifier, typeof(T));
        }

        T aggregate =
            GenericAggregateFactory<T>.CreateAggregate(aggregateDataModel?.AggregateIdentifier ?? id, identityContext);

        if (eventsData?.Any() != true)
        {
            throw new AggregateNotFoundException(typeof(T), aggregateDataModel?.AggregateIdentifier ?? id);
        }

        List<IDomainEvent> events = _domainEventMapper.Map(eventsData).ToList();

        aggregate.Rehydrate(events);

        return aggregate;
    }

    private async Task<T> RehydrateAsync<T>(Guid id, IIdentityContext identityContext,
        CancellationToken cancellationToken = default)
        where T : AggregateRoot
    {
        AggregateDataModel aggregateDataModel = await _aggregateStore.GetAsync(id, cancellationToken);
        
        IReadOnlyCollection<DomainEventDataModel>
            eventsData = await _aggregateStore.GetAsync(id, -1, cancellationToken);

        if (aggregateDataModel?.IsDeleted == true)
        {
            throw new AggregateDeletedException(aggregateDataModel.AggregateIdentifier, typeof(T));
        }

        T aggregate =
            GenericAggregateFactory<T>.CreateAggregate(aggregateDataModel?.AggregateIdentifier ?? id, identityContext);

        if (eventsData?.Any() != true)
        {
            throw new AggregateNotFoundException(typeof(T), aggregateDataModel?.AggregateIdentifier ?? id);
        }

        List<IDomainEvent> events = _domainEventMapper.Map(eventsData).ToList();

        aggregate.Rehydrate(events);

        return aggregate;
    }
}