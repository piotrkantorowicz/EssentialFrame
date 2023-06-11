namespace EssentialFrame.Cqrs.Commands.Persistence.Models;

public class CommandDataModel
{
    public virtual Guid CommandIdentifier { get; set; }

    public virtual object Command { get; set; }

    public virtual string CommandClass { get; set; }

    public virtual string CommandType { get; set; }

    public virtual DateTimeOffset? SendScheduled { get; set; }

    public virtual DateTimeOffset? SendStarted { get; set; }

    public virtual DateTimeOffset? SendCompleted { get; set; }

    public virtual DateTimeOffset? SendCancelled { get; set; }

    public virtual DateTimeOffset? CreatedAt { get; set; }

    public virtual string SendStatus { get; set; }

    public virtual string ExecutionStatus { get; set; }
}