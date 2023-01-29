using Microsoft.Extensions.Logging;

namespace EssentialFrame.Cqrs.Queries.Logging;

internal static class LoggingUtils
{
    internal static EventId QueryExecuting { get; } = new(2002, "QueryExecuting");

    internal static EventId QueryExecuted { get; } = new(2003, "QueryExecuted");

    internal static EventId UnexpectedException { get; } = new(2006, "UnexpectedException");
}