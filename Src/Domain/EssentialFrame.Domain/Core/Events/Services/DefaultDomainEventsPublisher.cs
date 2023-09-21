using Autofac;
using Autofac.Core;
using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.Events.Services.Interfaces;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Extensions;

namespace EssentialFrame.Domain.Core.Events.Services;

public class DefaultDomainEventsPublisher<TAggregateIdentifier> : IDomainEventsPublisher<TAggregateIdentifier>
    where TAggregateIdentifier : TypedGuidIdentifier
{
    private readonly ILifetimeScope _lifetimeScope;

    public DefaultDomainEventsPublisher(ILifetimeScope lifetimeScope)
    {
        _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
    }

    public void Publish<TDomainEvent>(TDomainEvent @event)
        where TDomainEvent : class, IDomainEvent<TAggregateIdentifier>
    {
        using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();

        IEventHandler<TDomainEvent, TAggregateIdentifier> handler =
            FindHandler<TDomainEvent, IEventHandler<TDomainEvent, TAggregateIdentifier>>(@event, scope);

        handler.Handle(@event);
    }

    public async Task PublishAsync<TDomainEvent>(TDomainEvent @event, CancellationToken cancellationToken = default)
        where TDomainEvent : class, IDomainEvent<TAggregateIdentifier>
    {
        await using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();

        IAsyncEventHandler<TDomainEvent, TAggregateIdentifier> handler =
            FindHandler<TDomainEvent, IAsyncEventHandler<TDomainEvent, TAggregateIdentifier>>(@event, scope);

        await handler.HandleAsync(@event, cancellationToken);
    }

    private static THandler FindHandler<TDomainEvent, THandler>(TDomainEvent @event, IComponentContext lifetimeScope)
        where TDomainEvent : class, IDomainEvent<TAggregateIdentifier> where THandler : class, IEventHandler
    {
        bool isTenantHandlerFound = lifetimeScope.TryResolveKeyed(@event.DomainEventIdentity.TenantIdentifier,
            out THandler eventHandler);

        if (isTenantHandlerFound)
        {
            return eventHandler;
        }

        bool isGeneralHandlerFound = lifetimeScope.TryResolve(out eventHandler);

        if (isGeneralHandlerFound)
        {
            return eventHandler;
        }

        throw new DependencyResolutionException($"Unable to resolve {@event.GetTypeFullName()}. " +
                                                "Most likely it is not properly registered in container");
    }
}