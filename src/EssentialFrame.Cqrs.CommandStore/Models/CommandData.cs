using EssentialFrame.Core;
using EssentialFrame.Core.Extensions;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.CommandStore.Const;
using EssentialFrame.Serialization;

namespace EssentialFrame.Cqrs.CommandStore.Models;

public abstract class CommandData
{
    public virtual Guid CommandIdentifier { get; }

    public virtual object Command { get; }

    public virtual string CommandType { get; }

    public virtual DateTimeOffset? SendScheduled { get; private set; }

    public virtual DateTimeOffset? SendStarted { get; private set; }

    public virtual DateTimeOffset? SendCompleted { get; private set; }

    public virtual DateTimeOffset? SendCancelled { get; private set; }

    public virtual DateTimeOffset? CreatedAt { get; }

    public virtual string SendStatus { get; set; }

    protected CommandData(Guid commandIdentifier, ICommand command)
    {
        CommandIdentifier = commandIdentifier;

        Validate(command, command.GetTypeName());

        CommandType = command.GetTypeName();
        Command = command;
        CreatedAt = SystemClock.Now;
    }

    protected CommandData(Guid commandIdentifier,
                          ICommand command,
                          ISerializer serializer)
    {
        CommandIdentifier = commandIdentifier;

        Validate(command, command.GetTypeName());

        CommandType = command.GetTypeName();
        Command = serializer.Serialize(command);
        CreatedAt = SystemClock.Now;
    }

    public virtual void Start()
    {
        ValidateExecutionStatus(CommandExecutionStatuses.Started);

        SendStarted = SystemClock.Now;
        SendStatus = CommandExecutionStatuses.Started;
    }

    public virtual void Schedule()
    {
        ValidateExecutionStatus(CommandExecutionStatuses.Scheduled);

        SendScheduled = SystemClock.Now;
        SendStatus = CommandExecutionStatuses.Scheduled;
    }

    public virtual void Cancel()
    {
        ValidateExecutionStatus(CommandExecutionStatuses.Cancelled);

        SendCancelled = SystemClock.Now;
        SendStatus = CommandExecutionStatuses.Cancelled;
    }

    public virtual void Complete()
    {
        ValidateExecutionStatus(CommandExecutionStatuses.Completed);

        SendCompleted = SystemClock.Now;
        SendStatus = CommandExecutionStatuses.Completed;
    }

    private static void ValidateExecutionStatus(string commandExecutionStatus)
    {
        if (commandExecutionStatus?.Length is < 3 or > 20)
        {
            throw new
                OverflowException($"Command execution status have to be longer than 3 characters and shorter than 20. Value: {commandExecutionStatus}");
        }
    }

    private static void Validate(ICommand command, string commandType)
    {
        if (command is null)
        {
            throw new ArgumentException("Command class cannot be null.", nameof(command));
        }

        if (commandType.Length is < 3 or > 100)
        {
            throw new
                OverflowException($"Command type have to be longer than 3 characters and shorter than 100. Value: {commandType}");
        }
    }
}
