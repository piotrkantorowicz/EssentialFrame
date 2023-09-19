using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.Events.Interfaces;

public interface IDomainEventsPublisher<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    void Publish<TDomainEvent>(TDomainEvent @event) where TDomainEvent : class, IDomainEvent<TAggregateIdentifier>;

    Task PublishAsync<TDomainEvent>(TDomainEvent @event, CancellationToken cancellationToken = default)
        where TDomainEvent : class, IDomainEvent<TAggregateIdentifier>;
}