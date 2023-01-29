namespace EssentialFrame.Cqrs.Events.Core.Interfaces;

public interface IAsyncEventHandler<in TEvent> : IEventHandler where TEvent : class, IEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}

public interface IEventHandler<in TEvent> : IEventHandler where TEvent : class, IEvent
{
    void Handle(TEvent @event);
}

public interface IEventHandler
{
}