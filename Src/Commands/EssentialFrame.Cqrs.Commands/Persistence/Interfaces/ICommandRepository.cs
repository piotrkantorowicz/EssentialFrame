using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Persistence.Interfaces;

internal interface ICommandRepository
{
    void StartExecution(ICommand command);

    Task StartExecutionAsync(ICommand command, CancellationToken cancellationToken);

    void StartExecution(ICommand command, ISerializer serializer);

    Task StartExecutionAsync(ICommand command, ISerializer serializer, CancellationToken cancellationToken);

    void CancelExecution(Guid commandIdentifier);

    Task CancelExecutionAsync(Guid commandIdentifier, CancellationToken cancellationToken);

    void ScheduleExecution(ICommand command, DateTimeOffset at);

    Task ScheduleExecutionAsync(ICommand command, DateTimeOffset at, CancellationToken cancellationToken);

    void CompleteExecution(Guid commandIdentifier, bool isSuccess);

    Task CompleteExecutionAsync(Guid commandIdentifier, bool isSuccess, CancellationToken cancellationToken);

    IReadOnlyCollection<ICommand> GetPossibleToSend(DateTimeOffset at);

    Task<IReadOnlyCollection<ICommand>> GetPossibleToSendAsync(DateTimeOffset at, CancellationToken cancellationToken);
}