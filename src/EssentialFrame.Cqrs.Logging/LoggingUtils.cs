using Microsoft.Extensions.Logging;

namespace EssentialFrame.Cqrs.Logging;

internal static class LoggingUtils
{
    internal static EventId CommandExecuting { get; } = new(2000, "CommandExecuting");

    internal static EventId CommandExecuted { get; } = new(2001, "CommandExecuted");

    internal static EventId QueryExecuting { get; } = new(2002, "QueryExecuting");

    internal static EventId QueryExecuted { get; } = new(2003, "QueryExecuted");

    internal static EventId ValidationFailed { get; } = new(2004, "ValidationFailed");

    internal static EventId ValidationSuccess { get; } = new(2005, "ValidationSuccess");

    internal static EventId UnexpectedException { get; } = new(2006, "UnexpectedException");
}
