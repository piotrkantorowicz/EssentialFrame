using EssentialFrame.Cqrs.Commands.Core.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Executors.Interfaces;

public interface ICommandScheduler
{
    void Schedule<TCommand>(TCommand command, DateTimeOffset at) where TCommand : class, ICommand;

    void CancelFromSchedule<TCommand>(TCommand command) where TCommand : class, ICommand;

    Task ScheduleAsync<TCommand>(TCommand command, DateTimeOffset at, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;

    Task CancelFromScheduleAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;
}