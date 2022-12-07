using EssentialFrame.Cqrs.Commands.Core;

namespace EssentialFrame.Cqrs.Interfaces;

public interface ICommandDispatcher
{
    ICommandResult Send<T>(T command)
        where T : class, ICommand;

    Task<ICommandResult> SendAsync<T>(T command, CancellationToken cancellationToken = default)
        where T : class, ICommand;

    ICommandResult Schedule<T>(T command, DateTimeOffset at)
        where T : class, ICommand;

    Task<ICommandResult> ScheduleAsync<T>(T command,
                                          DateTimeOffset at,
                                          CancellationToken cancellationToken = default)
        where T : class, ICommand;
}
