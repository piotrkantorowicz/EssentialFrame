using EssentialFrame.Cqrs.Commands.Errors.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Interfaces;

public interface ICommandResult
{
    bool IsSuccess { get; }

    object Data { get; }

    ICommandError ErrorDetails { get; }
}

