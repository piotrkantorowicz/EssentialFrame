namespace EssentialFrame.Cqrs.Commands.Core.Interfaces;

public interface ICommandRepository
{
    void StartExecution(ICommand command);

    Task StartExecutionAsync(ICommand command, CancellationToken cancellationToken = default);

    void CancelExecution(Guid commandIdentifier);

    Task CancelExecutionAsync(Guid commandIdentifier, CancellationToken cancellationToken = default);

    void ScheduleExecution(ICommand command, DateTimeOffset at);

    Task ScheduleExecutionAsync(ICommand command, DateTimeOffset at, CancellationToken cancellationToken = default);

    void CompleteExecution(Guid commandIdentifier, bool isSuccess);

    Task CompleteExecutionAsync(Guid commandIdentifier, bool isSuccess, CancellationToken cancellationToken = default);

    IReadOnlyCollection<ICommand> GetPossibleToSend(DateTimeOffset at);

    Task<IReadOnlyCollection<ICommand>> GetPossibleToSendAsync(DateTimeOffset at,
        CancellationToken cancellationToken = default);
}