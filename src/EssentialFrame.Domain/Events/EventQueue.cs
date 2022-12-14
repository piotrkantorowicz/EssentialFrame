using EssentialFrame.Core.Extensions;
using EssentialFrame.Domain.Core;
using EssentialFrame.Domain.Exceptions;

namespace EssentialFrame.Domain.Events;

public class EventQueue : IEventQueue
{
    private readonly Dictionary<(string, Guid), Action<IEvent>> _overriders;
    private readonly Dictionary<string, List<Action<IEvent>>> _subscribers;

    public EventQueue()
    {
        _subscribers = new Dictionary<string, List<Action<IEvent>>>();
        _overriders = new Dictionary<(string, Guid), Action<IEvent>>();
    }

    public void Publish(IEvent @event)
    {
        var name = @event.GetTypeFullName();

        if (_overriders.TryGetValue((name, @event.IdentityTenant), out var customization))
        {
            customization?.Invoke(@event);
        }
        else if (_subscribers.TryGetValue(name, out var actions))
        {
            foreach (var action in actions)
            {
                action.Invoke(@event);
            }
        }
        else
        {
            throw new UnhandledEventException(name);
        }
    }

    public void Subscribe<T>(Action<T> action)
        where T : IEvent
    {
        var name = typeof(T).FullName;

        if (_subscribers.All(x => x.Key != name))
        {
            _subscribers.Add(name, new List<Action<IEvent>>());
        }

        _subscribers[name].Add(@event => action((T)@event));
    }

    public void Override<T>(Action<T> action, Guid tenant)
        where T : IEvent
    {
        var name = typeof(T).AssemblyQualifiedName;

        if (_overriders.Any(x => x.Key.Item1 == name && x.Key.Item2 == tenant))
        {
            throw new AmbiguousEventHandlerException(name);
        }

        _overriders.Add((name, tenant), command => action((T)command));
    }
}
