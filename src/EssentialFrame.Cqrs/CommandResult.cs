using EssentialFrame.Cqrs.Errors.Core;
using EssentialFrame.Cqrs.Interfaces;

namespace EssentialFrame.Cqrs;

public sealed class CommandResult : ICommandResult
{
    private CommandResult()
    {
    }

    public bool Ok { get; private init; }

    public ICommandError ErrorDetails { get; private init; }

    public object Data { get; private init; }

    public static CommandResult Success() =>
        new()
        {
            Ok = true
        };

    public static CommandResult Success(object data) =>
        new()
        {
            Ok = true,
            Data = data
        };

    public static CommandResult Fail(ICommandError commandError) =>
        new()
        {
            Ok = false,
            ErrorDetails = commandError
        };
}
