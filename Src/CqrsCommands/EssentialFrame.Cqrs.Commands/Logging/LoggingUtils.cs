using Microsoft.Extensions.Logging;

namespace EssentialFrame.Cqrs.Commands.Logging;

internal static class LoggingUtils
{
    internal static EventId CommandExecuting { get; } = new(2000, "CommandExecuting");

    internal static EventId CommandExecuted { get; } = new(2001, "CommandExecuted");

    internal static EventId UnexpectedException { get; } = new(2006, "UnexpectedException");
}