using EssentialFrame.Cqrs.Commands.Core;

namespace EssentialFrame.Cqrs.Commands.Queues;

public interface ICommandQueue
{
    void Subscribe<T>(Action<T> action)
        where T : ICommand;

    void Override<T>(Action<T> action, Guid tenant)
        where T : ICommand;

    void Send(ICommand command, bool saveInStore);

    void Schedule(ICommand command, DateTimeOffset at);

    void Ping();

    void Start(Guid command);

    void Cancel(Guid command);

    void Complete(Guid command);
}
