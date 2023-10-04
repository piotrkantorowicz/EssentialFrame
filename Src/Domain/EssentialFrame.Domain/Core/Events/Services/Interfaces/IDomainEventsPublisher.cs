using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.Events.Services.Interfaces;

public interface IDomainEventsPublisher<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    void Publish<TDomainEvent>(TDomainEvent @event)
        where TDomainEvent : class, IDomainEvent<TAggregateIdentifier, TType>;

    Task PublishAsync<TDomainEvent>(TDomainEvent @event, CancellationToken cancellationToken = default)
        where TDomainEvent : class, IDomainEvent<TAggregateIdentifier, TType>;
}