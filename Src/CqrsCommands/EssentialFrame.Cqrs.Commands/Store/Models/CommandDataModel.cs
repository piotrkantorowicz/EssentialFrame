using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Store.Const;
using EssentialFrame.Extensions;
using EssentialFrame.Time;

namespace EssentialFrame.Cqrs.Commands.Store.Models;

public class CommandDataModel
{
    public CommandDataModel(ICommand command)
    {
        CommandIdentifier = command.CommandIdentifier;

        Validate(command, command.GetTypeFullName(), command.GetClassName());

        CommandClass = command.GetClassName();
        CommandType = command.GetTypeFullName();
        Command = command;
        CreatedAt = SystemClock.UtcNow;
    }

    public virtual Guid CommandIdentifier { get; }

    public virtual object Command { get; }

    public virtual string CommandClass { get; }

    public virtual string CommandType { get; }

    public virtual DateTimeOffset? SendScheduled { get; private set; }

    public virtual DateTimeOffset? SendStarted { get; private set; }

    public virtual DateTimeOffset? SendCompleted { get; private set; }

    public virtual DateTimeOffset? SendCancelled { get; private set; }

    public virtual DateTimeOffset? CreatedAt { get; }

    public virtual string SendStatus { get; private set; }

    public virtual string ExecutionStatus { get; private set; }

    public virtual void Start()
    {
        ValidateCommandSendStatuses(new[]
        {
            CommandSendingStatuses.Started, CommandExecutionStatuses.WaitingForExecution
        });

        SendStarted = SystemClock.UtcNow;
        SendStatus = CommandSendingStatuses.Started;
        ExecutionStatus = CommandExecutionStatuses.WaitingForExecution;
    }

    public virtual void Schedule(DateTimeOffset? at)
    {
        ValidateCommandSendStatuses(new[]
        {
            CommandSendingStatuses.Scheduled, CommandExecutionStatuses.WaitingForExecution
        });

        SendScheduled = at;
        SendStatus = CommandSendingStatuses.Scheduled;
        ExecutionStatus = CommandExecutionStatuses.WaitingForExecution;
    }

    public virtual void Cancel()
    {
        ValidateCommandSendStatuses(new[]
        {
            CommandSendingStatuses.Cancelled, CommandExecutionStatuses.ExecutionCancelled
        });

        SendCancelled = SystemClock.UtcNow;
        SendStatus = CommandSendingStatuses.Cancelled;
        ExecutionStatus = CommandExecutionStatuses.ExecutionCancelled;
    }

    public virtual void Complete(bool success)
    {
        ValidateCommandSendStatuses(new[]
        {
            CommandSendingStatuses.Completed, CommandExecutionStatuses.ExecutedSuccessfully,
            CommandExecutionStatuses.ExecutedWithErrors
        });

        SendCompleted = SystemClock.UtcNow;
        SendStatus = CommandSendingStatuses.Completed;

        ExecutionStatus = success
            ? CommandExecutionStatuses.ExecutedSuccessfully
            : CommandExecutionStatuses.ExecutedWithErrors;
    }

    private static void ValidateCommandSendStatuses(IEnumerable<string> commandStatuses)
    {
        foreach (string commandSendStatus in commandStatuses)
        {
            if (commandSendStatus?.Length is <= 3 or >= 30)
            {
                throw new OverflowException(
                    $"Command status have to be longer or equal than 3 characters and shorter or equal than 30. Value: {commandSendStatus}");
            }
        }
    }

    private static void Validate(ICommand command, string commandType, string commandClass)
    {
        if (command is null)
        {
            throw new ArgumentException("Command class cannot be null.");
        }

        if (commandType?.Length is <= 3 or >= 100)
        {
            throw new OverflowException(
                $"Command type have to be longer or equal than 3 characters and shorter or equal than 100. Value: {commandType}");
        }

        if (commandClass?.Length is <= 3 or >= 250)
        {
            throw new OverflowException(
                $"Command class have to be longer or equal than 3 characters and shorter or equal than 250. Value: {commandClass}");
        }
    }
}