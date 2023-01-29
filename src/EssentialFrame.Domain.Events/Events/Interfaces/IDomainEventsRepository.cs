﻿using EssentialFrame.Domain.Aggregates;

namespace EssentialFrame.Domain.Events.Events.Interfaces;

public interface IDomainEventsRepository
{
    T Get<T>(Guid id) where T : AggregateRoot;

    Task<T> GetAsync<T>(Guid id, CancellationToken cancellationToken = default) where T : AggregateRoot;

    IDomainEvent[] Save<T>(T aggregate, int? version = null) where T : AggregateRoot;

    Task<IDomainEvent[]> SaveAsync<T>(T aggregate, int? version = null, CancellationToken cancellationToken = default)
        where T : AggregateRoot;

    IDomainEvent ConvertToEvent(DomainEventDao domainEventDao);
}