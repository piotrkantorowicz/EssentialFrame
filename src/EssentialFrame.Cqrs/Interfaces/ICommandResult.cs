using EssentialFrame.Cqrs.Errors.Core;

namespace EssentialFrame.Cqrs.Interfaces;

public interface ICommandResult
{
    bool Ok { get; }

    object Data { get; }

    ICommandError ErrorDetails { get; }
}
