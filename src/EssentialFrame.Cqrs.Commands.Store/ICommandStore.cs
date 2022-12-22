using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Store.Models;

namespace EssentialFrame.Cqrs.Commands.Store;

public interface ICommandStore
{
    bool Exists(Guid commandIdentifier);

    Task<bool> ExistsAsync(Guid commandIdentifier, CancellationToken cancellationToken = default);

    CommandData Get(Guid commandIdentifier);

    Task<CommandData> GetAsync(Guid commandIdentifier, CancellationToken cancellationToken = default);

    IReadOnlyCollection<CommandData> GetPossibleToSend(DateTimeOffset at);

    Task<IReadOnlyCollection<CommandData>>
        GetPossibleToSendAsync(DateTimeOffset at, CancellationToken cancellationToken = default);

    void StartExecution(ICommand command);

    Task StartExecutionAsync(ICommand command, CancellationToken cancellationToken = default);

    void CancelExecution(Guid commandIdentifier);

    Task CancelExecutionAsync(Guid commandIdentifier, CancellationToken cancellationToken = default);

    void ScheduleExecution(ICommand command, DateTimeOffset at);

    Task ScheduleExecutionAsync(ICommand command,
                                DateTimeOffset at,
                                CancellationToken cancellationToken = default);

    void CompleteExecution(Guid commandIdentifier, bool isSuccess);

    Task CompleteExecutionAsync(Guid commandIdentifier,
                                bool isSuccess,
                                CancellationToken cancellationToken = default);

    ICommand ConvertToCommand(CommandData commandData);

    CommandData ConvertToCommandData(ICommand command);
}




