using EssentialFrame.Cqrs.Commands.Errors.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Core.Interfaces;

public interface ICommandResult
{
    bool IsSuccess { get; }

    object Data { get; }

    ICommandError ErrorDetails { get; }
}