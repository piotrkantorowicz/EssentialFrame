using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Persistence.Const;
using EssentialFrame.Cqrs.Commands.Persistence.Interfaces;
using EssentialFrame.Cqrs.Commands.Persistence.Models;
using EssentialFrame.Extensions;
using EssentialFrame.Serialization.Interfaces;
using EssentialFrame.Time;

namespace EssentialFrame.Cqrs.Commands.Persistence;

internal sealed class CommandDataModelService : ICommandDataModelService
{
    public CommandDataModel Create(ICommand command)
    {
        CommandDataModel commandDataModel = new();

        ValidateCommand(command);
        ValidateCommandType(command.GetTypeFullName());
        ValidateCommandClass(command.GetClassName());

        commandDataModel.CommandIdentifier = command.CommandIdentifier;
        commandDataModel.CommandClass = command.GetClassName();
        commandDataModel.CommandType = command.GetTypeFullName();
        commandDataModel.Command = command;
        commandDataModel.CreatedAt = SystemClock.UtcNow;

        return commandDataModel;
    }

    public CommandDataModel Create(ICommand command, ISerializer serializer)
    {
        CommandDataModel commandDataModel = new();

        ValidateCommand(command);
        ValidateCommandType(command.GetTypeFullName());
        ValidateCommandClass(command.GetClassName());

        commandDataModel.CommandIdentifier = command.CommandIdentifier;
        commandDataModel.CommandClass = command.GetClassName();
        commandDataModel.CommandType = command.GetTypeFullName();
        commandDataModel.Command = serializer.Serialize(command);
        commandDataModel.CreatedAt = SystemClock.UtcNow;

        return commandDataModel;
    }

    public void Start(CommandDataModel commandDataModel)
    {
        ValidateCommandSendStatuses(new[]
        {
            CommandSendingStatuses.Started, CommandExecutionStatuses.WaitingForExecution
        });

        commandDataModel.SendStarted = SystemClock.UtcNow;
        commandDataModel.SendStatus = CommandSendingStatuses.Started;
        commandDataModel.ExecutionStatus = CommandExecutionStatuses.WaitingForExecution;
    }

    public void Schedule(CommandDataModel commandDataModel, DateTimeOffset? at)
    {
        ValidateCommandSendStatuses(new[]
        {
            CommandSendingStatuses.Scheduled, CommandExecutionStatuses.WaitingForExecution
        });

        commandDataModel.SendScheduled = at;
        commandDataModel.SendStatus = CommandSendingStatuses.Scheduled;
        commandDataModel.ExecutionStatus = CommandExecutionStatuses.WaitingForExecution;
    }

    public void Cancel(CommandDataModel commandDataModel)
    {
        ValidateCommandSendStatuses(new[]
        {
            CommandSendingStatuses.Cancelled, CommandExecutionStatuses.ExecutionCancelled
        });

        commandDataModel.SendCancelled = SystemClock.UtcNow;
        commandDataModel.SendStatus = CommandSendingStatuses.Cancelled;
        commandDataModel.ExecutionStatus = CommandExecutionStatuses.ExecutionCancelled;
    }

    public void Complete(CommandDataModel commandDataModel, bool success)
    {
        ValidateCommandSendStatuses(new[]
        {
            CommandSendingStatuses.Completed, CommandExecutionStatuses.ExecutedSuccessfully,
            CommandExecutionStatuses.ExecutedWithErrors
        });

        commandDataModel.SendCompleted = SystemClock.UtcNow;
        commandDataModel.SendStatus = CommandSendingStatuses.Completed;

        commandDataModel.ExecutionStatus = success
            ? CommandExecutionStatuses.ExecutedSuccessfully
            : CommandExecutionStatuses.ExecutedWithErrors;
    }

    private static void ValidateCommand(ICommand command)
    {
        if (command is null)
        {
            throw new ArgumentException("Command class cannot be null");
        }
    }

    private static void ValidateCommandType(string commandType)
    {
        if (commandType?.Length is <= 3 or >= 260)
        {
            throw new OverflowException(
                $"Command type have to be longer or equal than 3 characters and shorter or equal than 100. Value: {commandType}");
        }
    }

    private static void ValidateCommandClass(string commandClass)
    {
        if (commandClass?.Length is <= 3 or >= 260)
        {
            throw new OverflowException(
                $"Command class have to be longer or equal than 3 characters and shorter or equal than 250. Value: {commandClass}");
        }
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
}