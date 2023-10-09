using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.Events.Interfaces;

public interface IAsyncEventHandler<in TDomainEvent, TAggregateIdentifier, TType> : IEventHandler
    where TDomainEvent : class, IDomainEvent<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    Task HandleAsync(TDomainEvent @event, CancellationToken cancellationToken);
}

public interface IEventHandler<in TDomainEvent, TAggregateIdentifier, TType> : IEventHandler
    where TDomainEvent : class, IDomainEvent<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    void Handle(TDomainEvent @event);
}

public interface IEventHandler
{
}