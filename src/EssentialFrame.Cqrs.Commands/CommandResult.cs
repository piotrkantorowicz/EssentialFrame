using EssentialFrame.Cqrs.Commands.Errors.Interfaces;
using EssentialFrame.Cqrs.Commands.Interfaces;

namespace EssentialFrame.Cqrs.Commands;

public sealed class CommandResult : ICommandResult
{
    private CommandResult()
    {
    }

    public bool IsSuccess { get; private init; }

    public ICommandError ErrorDetails { get; private init; }

    public object Data { get; private init; }

    public static CommandResult Success() =>
        new()
        {
            IsSuccess = true
        };

    public static CommandResult Success(object data) =>
        new()
        {
            IsSuccess = true,
            Data = data
        };

    public static CommandResult Fail(ICommandError commandError) =>
        new()
        {
            IsSuccess = false,
            ErrorDetails = commandError
        };
}

