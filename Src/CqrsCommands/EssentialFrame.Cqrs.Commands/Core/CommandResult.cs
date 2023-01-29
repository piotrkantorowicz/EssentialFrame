using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Errors.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Core;

public sealed class CommandResult : ICommandResult
{
    private CommandResult()
    {
    }

    public bool IsSuccess { get; private init; }

    public ICommandError ErrorDetails { get; private init; }

    public object Data { get; private init; }

    public static CommandResult Success()
    {
        return new CommandResult { IsSuccess = true };
    }

    public static CommandResult Success(object data)
    {
        return new CommandResult { IsSuccess = true, Data = data };
    }

    public static CommandResult Fail(ICommandError commandError)
    {
        return new CommandResult { IsSuccess = false, ErrorDetails = commandError };
    }
}