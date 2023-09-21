﻿using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.Events.Interfaces;

public interface IAsyncEventHandler<in TDomainEvent, TAggregateIdentifier> : IEventHandler
    where TDomainEvent : class, IDomainEvent<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    Task HandleAsync(TDomainEvent @event, CancellationToken cancellationToken = default);
}

public interface IEventHandler<in TDomainEvent, TAggregateIdentifier> : IEventHandler
    where TDomainEvent : class, IDomainEvent<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    void Handle(TDomainEvent @event);
}

public interface IEventHandler
{
}