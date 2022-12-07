using EssentialFrame.Core;
using EssentialFrame.Cqrs.Commands.Const;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Exceptions;

namespace EssentialFrame.Cqrs.Commands.Queues;

internal sealed class CommandQueue : ICommandQueue
{
    private readonly Dictionary<(string, Guid), Action<ICommand>> _overriders;
    private readonly ICommandStore _store;
    private readonly Dictionary<string, Action<ICommand>> _subscribers;

    public CommandQueue(ICommandStore store)
    {
        _subscribers = new Dictionary<string, Action<ICommand>>();
        _overriders = new Dictionary<(string, Guid), Action<ICommand>>();
        _store = store;
    }

    // public void Subscribe<T>(Action<T> action)
    //     where T : ICommand
    // {
    //     var name = typeof(T).AssemblyQualifiedName ?? string.Empty;
    //
    //     if (_subscribers.Any(x => x.Key == name))
    //     {
    //         throw new AmbiguousCommandHandlerException(name);
    //     }
    //
    //     _subscribers.Add(name, command => action((T)command));
    // }
    //
    // public void Override<T>(Action<T> action, Guid tenant)
    //     where T : ICommand
    // {
    //     var name = typeof(T).AssemblyQualifiedName;
    //
    //     if (_overriders.Any(x => x.Key.Item1 == name && x.Key.Item2 == tenant))
    //     {
    //         throw new AmbiguousCommandHandlerException(name);
    //     }
    //
    //     _overriders.Add((name, tenant), command => action((T)command));
    // }

    public void Send(ICommand command, bool saveInStore = false)
    {
        SerializedCommand serialized = null;

        if (saveInStore)
        {
            serialized = _store.Serialize(command);
            serialized.SendStarted = SystemClock.Now;
        }

        Execute(command, command.GetType().AssemblyQualifiedName);

        if (saveInStore)
        {
            serialized.SendCompleted = SystemClock.Now;
            serialized.SendStatus = CommandExecutionStatuses.Completed;

            _store.Save(serialized, true);
        }
    }

    public void Schedule(ICommand command, DateTimeOffset at)
    {
        var serialized = _store.Serialize(command);

        serialized.SendScheduled = at;
        serialized.SendStatus = CommandExecutionStatuses.Scheduled;

        _store.Save(serialized, true);
    }

    public void Ping()
    {
        var commands = _store.GetExpired(SystemClock.Now);

        foreach (var command in commands)
        {
            Execute(command);
        }
    }

    public void Start(Guid command)
    {
        Execute(_store.Get(command));
    }

    public void Cancel(Guid command)
    {
        var serialized = _store.Get(command);
        serialized.SendCancelled = SystemClock.Now;
        serialized.SendStatus = CommandExecutionStatuses.Cancelled;

        _store.Save(serialized, false);
    }

    public void Complete(Guid command)
    {
        var serialized = _store.Get(command);
        serialized.SendCompleted = SystemClock.Now;
        serialized.SendStatus = CommandExecutionStatuses.Completed;

        _store.Save(serialized, false);
    }

    private void Execute(ICommand command, string @class)
    {
        if (_overriders.TryGetValue((@class, command.IdentityTenant), out var customization))
        {
            customization.Invoke(command);
        }
        else if (_subscribers.TryGetValue(@class, out var action))
        {
            action.Invoke(command);
        }
        else
        {
            throw new UnhandledCommandException(@class);
        }
    }

    private void Execute(SerializedCommand serialized)
    {
        serialized.SendStarted = SystemClock.Now;
        serialized.SendStatus = CommandExecutionStatuses.Started;

        _store.Save(serialized, false);

        Execute(serialized.Deserialize(_store.Serializer), serialized.CommandClass);

        serialized.SendCompleted = SystemClock.Now;
        serialized.SendStatus = CommandExecutionStatuses.Completed;

        _store.Save(serialized, false);
    }
}
